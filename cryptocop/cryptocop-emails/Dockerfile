FROM python:3

LABEL maintainer="svanhildur19@ru.is"

ENV PYTHON_ENV production

WORKDIR /app
COPY . /app

RUN pip3 install -r requirements.txt

EXPOSE 5001
CMD python3 -u ./app.py