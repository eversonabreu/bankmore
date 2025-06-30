# Instruções

Executar o comando:

docker compose up --build

(deve ser executado no diretório onde está o arquivo docker-compose.yaml)

Será criado 1 container para cada API, e mais 3 containers para o Kafka, Kafka-UI e outras ferramentas para o Kafka

Os bancos de dados de cada API é criado automaticamente com Migrations do Entity Framework

Para logar (gerar o token JWT), use o endpoint de login na API de conta-corrente
Deixei um conta já criada para facilitar a demonstração/uso. Use o payload abaixo para gerar o token:

{
  "code" : "1234",
  "password" : "1234"
}