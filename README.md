# Microsserviços - Email Service

Este é um exemplo de uma API + Worker + RabbitMQ + ElephantSQL

## Início Rápido

Para rodar este projeto é necessário:
- Rodar o Docker Compose
- Estar com o IP na Whitelist do KV da Azure

## Fluxo
Envia qual email será enviado pela API -> API produz a mensagem para a fila do RabbitMQ -> Worker consome a mensagem e faz o envio do e-mail -> Salva no banco ElephantSQL o log desse envio.
