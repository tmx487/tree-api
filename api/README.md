# Tree API

### 1\. Clone the repository

```bash
git clone https://github.com/solara999/tree-api.git
```

### 2\. Launch the containers

The setup automatically provisions the PostgreSQL database and applies all EF Core migrations upon the first startup.

```bash
docker-compose up --build
```

### 3\. Access the API

The application will be running on port `8080`.

| Resource | Address |
| :--- | :--- |
| **Base URL** | `http://localhost:8080` |
| **Swagger UI** | `http://localhost:8080/swagger` |