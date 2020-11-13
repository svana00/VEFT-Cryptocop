import pika
import requests
import json
from time import sleep
from os import environ

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
queue_name = "email-queue"
introduction_template = "<h2>Thanks for you order!</h2><p>We hope you will enjoy our lovely products and don't hesitate to contact us if there are any questions.</p>"
info_template = "<h3>Order details</h3>%s"
items_table = '<table><thead><tr style="background-color: rgba(155, 155, 155, .2)"><th>Product Identifier</th><th>Quantity</th><th>Unit price</th><th>Total Price</th></tr></thead><tbody>%s</tbody></table>'


def setup_queue(exchange_name, queue_name, routing_key):
    # Declare the queue, if it doesn't exist
    channel.queue_declare(queue=queue_name, durable=True)
    # Bind the queue to a specific exchange with a routing key
    channel.queue_bind(
        exchange=exchange_name, queue=queue_name, routing_key=routing_key
    )


# Declare the exchange, if it doesn't exist
channel.exchange_declare(exchange=exchange_name, exchange_type="direct", durable=True)

setup_queue(exchange_name, queue_name, create_order_routing_key)


def send_simple_message(to, subject, body):
    return requests.post(
        "https://api.mailgun.net/v3/sandbox4e22cbf8770b4aa9ad9dde67f439b1e2.mailgun.org/messages",
        auth=("api", "0ae21ca72b11aed9d11e516f1621fa39-53c13666-f4b868c4"),
        data={
            "from": "Mailgun Sandbox <postmaster@sandbox4e22cbf8770b4aa9ad9dde67f439b1e2.mailgun.org>",
            "to": to,
            "subject": subject,
            "html": body,
        },
    )


def send_ack(ch, delivery_tag, success):
    if success:
        ch.basic_ack(delivery_tag)


def send_order_email(ch, method, properties, data):
    parsed_msg = json.loads(data)
    email = parsed_msg["email"]
    items = parsed_msg["orderItems"]

    info_html = "<p>Full name: %s<br>Street name: %s<br>House number: %s<br>City: %s<br>Zip code: %s<br>Country: %s<br>Order date: %s<br>Total price: %s" % (
        parsed_msg["fullName"],
        parsed_msg["streetName"],
        parsed_msg["houseNumber"],
        parsed_msg["city"],
        parsed_msg["zipCode"],
        parsed_msg["country"],
        parsed_msg["orderDate"],
        float(parsed_msg["totalPrice"]),
    )

    items_html = "".join(
        [
            "<tr><td>%s</td><td>%.2f</td><td>%.2f</td><td>%d</td></tr>"
            % (
                item["productIdentifier"],
                item["quantity"],
                float(item["unitPrice"]),
                float(item["totalPrice"]),
            )
            for item in items
        ]
    )

    representation = (
        introduction_template + (info_template % info_html) + (items_table % items_html)
    )
    response = send_simple_message(email, "Cryptocop order confirmed.", representation)
    send_ack(ch, method.delivery_tag, response.ok)


channel.basic_consume(queue_name, send_order_email)


channel.start_consuming()
connection.close()
