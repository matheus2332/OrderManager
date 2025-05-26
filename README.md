# Order Manager

Aplicação para gerenciamento de pedidos, com backend em Node.js + TypeScript e frontend em React + Vite.

O objetivo é fazer um pedido (order) com uma determinada quantidade, limitado ao estoque de (100) como foi definido,
efetuando primeiramente uma "reserva" e posteriormente uma confirmação.
Entretanto se caso a confirmação da reserva não for confirmada no tempo maximo de 1 minuto, a quantidade reservada será retornada ao estoque.

> Defini este tempo de 1 minuto para facilitarmos os testes entretanto, isso pode ser facilmente alterado no ficheiro [InMemoryReservationPolicy.cs](http://google.com/)
 ```c#
public class InMemoryReservationPolicy : IReservationPolicy
{
    public TimeSpan GetReservationTime()
    {
        return TimeSpan.FromMinutes(1);
    }
}
```

# Solução relacionada ao tempo de reserva
Para solucionar o tema da reserva, usei o RabbitMQ para gerenciar as mensagens.
Tenho basicamente 2 filas ```order-expiration``` e ```order-expirated```, um publisher é chamado ao criar a "order", e publica na fila ```order-expiration```,
esta fila tem uma "BasicProperty" chamada "Expiration" que recebe um timeSpam (que pode vir de qualquer sitio, inclusive de uma futura base de dados), 
e um "argument" ```x-dead-letter-exchange``` usando um o conceito de  ```TTL (Time-To-Live)``` e  ```DLX (Dead Letter Exchanges)``` que define que, se esta mensagem caso nao for consumida no tempo definido (10 minutos), esta publica em uma segunda fila ```order-expirated```.
Esta fila então possui um "consumer" que irá verificar se o pedido foi confirmado, e caso não, retornará a quantidade ao Estoque e liberará as quantidades reservadas.

### 🧭 Conceito: TTL + DLX
> - Ao criar a order, uma mensagem é enviada para uma fila com TTL igual ao tempo de reserva (10 minutos).
> - Se a encomenda for concluída antes do tempo, nada acontece, simplesmente ignoramos a mensagem no RabbitMQ.
> - Se o TTL expira, a mensagem é movida automaticamente para uma Dead Letter Queue.
> - Um consumidor dessa fila lê a mensagem e libera o estoque e a reserva.

---

## 🔧 Requisitos

- Node.js >= 18
- Docker + Docker Compose
- npm ou pnpm

---

## 🚀 Backend (.Net via Docker)

1. No diretorio principal ```cd OrderManager```, suba o backend com Docker Compose:

```bash
docker compose up -d --build
```

2. A API estará disponível em: http://localhost:5009

Podes acender http://localhost:5009/swagger para ter os endpoints.

## 🚀 Backend com RabbitMQ (sem Docker)

1. RabbitMQ rodando localmente (pode usar Docker ou instalação nativa)
> Se quiser usar Docker para o RabbitMQ:

```bash
docker run -d --hostname rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```
2. Acesse a pasta do projeto backend .NET:
```bash
cd dotnet-backend
```
3. Restaure pacotes:
```bash
dotnet restore
```

4. Complie o projeto
```bash
dotnet build
```
5. Execute a app
```bash
dotnet run
```

## 🌐 Frontend (React)

1. Vá até a pasta frontend:

```bash
cd frontend/order-manager-ts
```

2. Instale as dependências:

```bash
npm install
```

3. Rode o servidor de desenvolvimento:

```bash
npm run dev
```

4. Acesse o app em: http://localhost:5173
> ⚠️ Certifique-se que o frontend está configurado para fazer chamadas para http://localhost:5009 ou ajuste a "const API_BASE" no OrderManager.tsx .



## ⚙️ Scripts úteis
Backend

Subir containers:

```bash
docker compose up -d
```
Parar containers:
```bash
docker compose down
```

Ver logs do backend:
```bash
docker logs -f order-manager-backend
```

## 📁 Estrutura do Projeto

```css
├── OrderManager.API
├── OrderManager.Application
├── OrderManager.Domain
├── OrderManager.Infrastructure
├── OrderManager.Tests
├── frontend
│   ├── order-manager-ts
│   │   ├── src
│   │   └── vite.config.ts
├── docker-compose.yml
└── README.md
```

## 📌 Notas
O frontend e backend rodam em portas diferentes.

Durante o desenvolvimento, o frontend se comunica diretamente com o backend local.






