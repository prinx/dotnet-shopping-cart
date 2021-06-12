# Dotnet Shopping Cart

Example of Shopping Cart API using .NET Core

This project assumes a user can have only one cart.

## API Documentation


### Add a new user

```http
POST http://localhost:5000/api/users
Content-type: application/json

{
    "name": "Caroline",
    "phoneNumber": "233..."
}
```

### Add product

```http
POST http://localhost:5000/api/products
Content-type: application/json

{
    "name": "Keyboard",
    "price": 5000,
    "inStock": true
}
```
6
### Add item to cart

```http
POST http://localhost:5000/api/cartItems
Content-type: application/json

{
    "ProductId": 3,
    "UserId": 2,
    "Quantity": 1
}
```

- Quantity is optional and is 1 by default
- A new cart item will be created if it does not exist
- Only cart item quantity will be updated if cart item already exist

### Delete cart item

```http
DELETE http://localhost:5000/api/cartItems
Content-type: application/json

{
    "ProductId": 3,
    "UserId": 2,
    "Quantity": 1
}
```

### Get cart items

```http
GET http://localhost:5000/api/cartItems
```

## Swagger link

<http://localhost:5000/swagger/index.html>

## LICENSE

[MIT](LICENSE)
