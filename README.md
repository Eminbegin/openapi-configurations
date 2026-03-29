# Configuration Service (Labs 2-3)

Сервис конфигураций с двумя методами:
- `POST /api/configurations` - сохранить/обновить набор конфигураций
- `GET /api/configurations` - получить конфигурации постранично

OpenAPI-спецификация: [`openapi.yaml`](docs/openapi.yaml)

## Требования
- .NET SDK 10
- Docker + Docker Compose

## Lab 2: Локальный запуск API
```bash
dotnet restore
dotnet run --project ./src/Configurations/Configurations.csproj
```

- Swagger: [http://localhost:5215/swagger/index.html](http://localhost:5215/swagger/index.html)

### Скриншоты API
- Swagger страница  
  ![Swagger UI](docs/images/lab2/swagger.png)
- `POST /api/configurations`  
  ![POST](docs/images/lab2/post.png)
- `GET /api/configurations`  
  ![GET](docs/images/lab2/get.png)

## Lab 3: Метрики и Grafana

### Добавленные custom product metrics
- `configurations_set_requests_total` - количество запросов на запись конфигураций
- `configurations_get_requests_total` - количество запросов на чтение конфигураций
- `configurations_entries_written_total` - сколько конфигураций записано через `POST`
- `configurations_entries_read_total` - сколько конфигураций отдано через `GET`
- `configurations_set_batch_size` - распределение размера батча записываемых конфигураций
- `configurations_get_result_size` - распределение размера ответа для чтения конфигураций

Дополнительно включены стандартные HTTP-метрики через `prometheus-net.AspNetCore`, а endpoint метрик доступен по `GET /metrics`.

### Инфраструктура (docker-compose)
Поднимаются сервисы:
- `postgres` - база данных приложения
- `app` - API сервиса конфигураций
- `victoriametrics` - time-series хранилище
- `vmagent` - scrape метрик и remote write в VictoriaMetrics
- `grafana` - дашборды (provisioning datasource + dashboards)

Запуск:
```bash
docker compose up --build
```

Полезные URL:
- API Swagger: [http://localhost:5215/swagger/index.html](http://localhost:5215/swagger/index.html)
- Метрики приложения: [http://localhost:5215/metrics](http://localhost:5215/metrics)
- VictoriaMetrics: [http://localhost:8428](http://localhost:8428)
- Grafana: [http://localhost:3000](http://localhost:3000) (`admin` / `admin`)

### Grafana dashboard
Provisioned dashboard: `Configuration Service - Lab 3 Metrics` (folder `LR3`).

Примеры PromQL/MetricQL-запросов, используемых в панелях:
- `sum(rate(configurations_set_requests_total[5m]))`
- `sum(rate(configurations_get_requests_total[5m]))`
- `sum(increase(configurations_entries_written_total[1h]))`
- `sum(increase(configurations_entries_read_total[1h]))`
- `histogram_quantile(0.95, sum(rate(configurations_set_batch_size_bucket[5m])) by (le))`
- `histogram_quantile(0.95, sum(rate(configurations_get_result_size_bucket[5m])) by (le))`

### Скриншоты

![grafana-panel-rps-edit](docs/images/lab3/grafana-panel-rps-edit.png)

![grafana-panel-set-batch-edit](docs/images/lab3/grafana-panel-set-batch-edit.png)

![grafana-panel-get-size-edit](docs/images/lab3/grafana-panel-get-size-edit.png)