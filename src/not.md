# Elasticsearch

- Veriler arasýnda arama yapmayý kolaylaþtýran bir teknolojidir.

## Elasticsearch için docker-compose iþlemi

- docker-compose.yml dosyasýna, **Elasticsearch** ve **Kibana** ile ilgili container bilgileri eklenir.
- Bu container'larýn birbirine dependency_on field'ý ile ve Elasticsearch URL'i ile baðlýlýðý saðlanýr.
- Volume'lerin, yani bu iki teknolojinin depolama konumlarýnýn doðru seçilmesi önemli