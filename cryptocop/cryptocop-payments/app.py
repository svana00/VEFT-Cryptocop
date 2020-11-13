import pika
import json
from time import sleep
from os import environ
from credit_card_checker import CreditCardChecker

connection = None


def get_connection_string():
    if environ.get("PYTHON_ENV") == None:
        with open("./config/mb.development.json", "r") as f:
            return json.load(f)
    else:
        with open("./config/mb.%s.json" % environ.get("PYTHON_ENV"), "r") as f:
            return json.load(f)


def connect_to_mb():
    error = False
    connection_string = get_connection_string()
    while not error:
        try:
            credentials = pika.PlainCredentials(
                connection_string["user"], connection_string["password"]
            )
            connection = pika.BlockingConnection(
                pika.ConnectionParameters(
                    connection_string["host"],
                    5672,
                    connection_string["virtualhost"],
                    credentials,
                )
            )
            channel = connection.channel()
            return channel
        except:
            sleep(5)
            continue


channel = connect_to_mb()

# Configuration
exchange_name = "cryptocop_exchange"
create_order_routing_key = "create-order"
payment_queue_name = "payment-queue"


def setup_queue(exchange_name, queue_name, routing_key):
    # Declare the queue, if it doesn't exist
    channel.queue_declare(queue=queue_name, durable=True)
    # Bind the queue to a specific exchange with a routing key
    channel.queue_bind(
        exchange=exchange_name, queue=queue_name, routing_key=routing_key
    )


# Declare the exchange, if it doesn't exist
channel.exchange_declare(exchange=exchange_name, exchange_type="direct", durable=True)

setup_queue(exchange_name, payment_queue_name, create_order_routing_key)


def print_validation_message(bool):
    # Print out the validation message in the console
    if bool == True:
        print("Credit card has been validated.")
    else:
        print("Credit card declined.")


def validate_credit_card(ch, method, properties, data):
    parsed_msg = json.loads(data)
    card_num = parsed_msg["creditCard"]

    if CreditCardChecker(card_num).valid():
        print_validation_message(True)
    else:
        print_validation_message(False)


channel.basic_consume(payment_queue_name, validate_credit_card)


channel.start_consuming()
connection.close()
