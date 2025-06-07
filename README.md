# tree-api

## deploying

1. clone repo

```bash
git clone https://github.com/tmx487/tree-api.git
cd tree-api
```

2. add postgresql connection string in `appsettings.Development.json`

```bash
 "ConnectionStrings": {
    "PostgreSQl": "Host=localhost;Port=9432;Database=tree_db;Username=tree_dba;Password=tree_dba_21278;"
  }
```

3. apply migrations

```bash
dotnet ef database update
```

4. run API

```bash
dotnet run --launch-profile https
```
API is running on ports:
- https://localhost:7220
- http://localhost:5145

Swagger is also available on route `/swagger`.
## testing

run tests

```bash
dotnet test
```