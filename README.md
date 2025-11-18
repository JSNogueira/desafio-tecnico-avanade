# Desafio Tecnico Avanade

## Sistema de Vendas e Estoque — Microservices (.NET 9 + Docker + RabbitMQ + API Gateway + Blazor WASM)

Este projeto é uma arquitetura completa de microserviços construída com .NET 9, Blazor WebAssembly, RabbitMQ, MySQL e API Gateway Ocelot, totalmente orquestrado via Docker Compose.

# Como Executar o Projeto
## Pré-requisitos

Docker + Docker Compose

.NET 9 (caso deseje rodar serviços fora do docker)


## Subir todo o sistema
```bash
docker compose up --build
```
Esse comando cria todos os containers necessários para o funcionamento da aplicação completa. 
As migrations são executadas automaticamente criando todas as bases de dados e os seguintes usuários:

| Usuário            | Acesso                                                                       |
| ------------------ | ---------------------------------------------------------------------------- |
| Administrador      | Email: admin@mail.com - Senha: admin123                                      |
| Cliente1           | Email: cliente1@mail.com - Senha: cliente123                                 |
| Cliente2           | Email: cliente2@mail.com - Senha: cliente456                                 |

## Acessar os serviços
| Serviço            | URL                                                                          |
| ------------------ | ---------------------------------------------------------------------------- |
| Frontend Blazor    | [http://localhost:5088](http://localhost:5088)                               |
| API Gateway        | [http://localhost:5000](http://localhost:5000)                               |
| RabbitMQ Dashboard | [http://localhost:15672](http://localhost:15672) (user: guest / pass: guest) |
| Logs Seq           | [http://localhost:5341](http://localhost:5341) (user: admin / pass: admin123)|


# Tecnologias Utilizadas
## Back-end

* .NET 9
* ASP.NET Web API
* Entity Framework Core
* MySQL
* RabbitMQ
* Ocelot API Gateway
* JWT (JSON Web Token) para autenticação

## Front-end

* Blazor WebAssembly
* Blazored.LocalStorage (armazenamento do token)
* CustomAuthStateProvider para validar sessão
* Consumo das APIs via Gateway

## Infraestrutura

* **Docker Compose contendo:**
  * Microserviço de Estoque
  * Microserviço de Vendas
  * Banco MySQL
  * RabbitMQ
  * API Gateway
  * Frontend Blazor
