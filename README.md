# Dotnet Shopping Cart

Example of Shopping Cart API using .NET Core

This project assumes a user can have only one cart.

## API Documentation

### Add item to cart

```http
POST https://localhost:{port}/api/cartItems
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
DELETE https://localhost:{port}/api/cartItems
Content-type: application/json

{
    "ProductId": 3,
    "UserId": 2,
    "Quantity": 1
}
```

### Get cart items

```http
GET https://localhost:{port}/api/cartItems
```

### Add a new user

```http
POST https://localhost:{port}/api/users
Content-type: application/json

{
    "name": "Caroline",
    "phoneNumber": "233..."
}
```

### Add product

```http
POST https://localhost:{port}/api/products
Content-type: application/json

{
    "name": "Keyboard",
    "price": 5000,
    "inStock": true
}
```

## LICENSE

[MIT](LICENSE)
