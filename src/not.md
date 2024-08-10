# Elasticsearch

- Veriler aras�nda arama yapmay� kolayla�t�ran bir teknolojidir.

## Elasticsearch i�in docker-compose i�lemi

- docker-compose.yml dosyas�na, **Elasticsearch** ve **Kibana** ile ilgili container bilgileri eklenir.
- Bu container'lar�n birbirine dependency_on field'� ile ve Elasticsearch URL'i ile ba�l�l��� sa�lan�r.
- Volume'lerin, yani bu iki teknolojinin depolama konumlar�n�n do�ru se�ilmesi �nemli