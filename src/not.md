# Elasticsearch

- Veriler arasında arama yapmayı kolaylaştıran bir teknolojidir.

## Elasticsearch için docker-compose işlemi

- docker-compose.yml dosyasına, **Elasticsearch** ve **Kibana** ile ilgili container bilgileri eklenir.
- Bu container'ların birbirine dependency_on field'ı ile ve Elasticsearch URL'i ile bağlılığı sağlanır.
- Volume'lerin, yani bu iki teknolojinin depolama konumlarının doğru seçilmesi önemli
- Orchestration işleminden sonra Elasticsearch ve Kibana'ya ilgili URL'lerinden erişebiliriz.

## Elasticsearch API

### Creating Document

- Kibana Console'dan yahut herhangi bir client üzerinden istek atabiliriz.
- **PUT** keyword'ü ile istediğimiz uzantıyı da seçerek doküman oluşturabiliriz. (JSON data yazar gibi)
- Data => Document
- Column => Field
- Table => Index

```
PUT products/_doc/1
{
	"name": "iPhone 6",
	"rating": 100,
	"published": true,
	"category": "mobile phone"
	"price": {
	   "usd": 3000,
	   "eur": 2500
	}
}
```

- Bu kullanım, **Dynamic Mapping** olarak adlandırılır. Veri tipini belirtmediğimiz ve Elasticsearch'ün kendisi çıkarım yaptığı durum.
- Dönen response;

```
{
	"_index": "products", (hangi index'e kaydedildi?)
	"_id": "1", (Document ID)
	"_version": 1, (her update işleminden sonra artırılır)
	"result": "created",
	"_shards": {
	   "total": 2,
	   "successful": 1,
	   "failed": 0
	},
	"_seq_no": 0, (her bir operasyonda artar)
	"_primary_term": 1
}
```

### Retrieving Document

- **GET** isteği ile ilgili index'i ve ID'yi seçerek hedef document'ı getirebiliriz.
- _doc yerine _source kullanırsak, sadece source field'ını elde ederiz.

```
GET products/_doc/1 

=>

{
	"_index": "products",
	"_id": "1",
	"_version": 1,
	"_seq_no": 0,
	"_primary_term": 1,
	"found": true, (buraya kadar olan kısım metadata bilgileri)
	"_source": { (asıl datamızı içeren field)
		"name": "iPhone 6",
		"rating": 100,
		"published": true,
		"category": "mobile phone"
		"price": {
		   "usd": 3000,
		   "eur": 2500
		}
	}
}
```

### Retrieving Shards

- **GET** isteği ile hedef index'e ait shard bilgilerine erişebiliriz.

```
GET _cat/shards/products

=>

products 0 p STARTED
products 0 r UNASSIGNED
```

- Response'u değerlendirecek olursak, iki satır döndürdü. İlk satır primary, ikinci satır ise replica shard'a işaret eder

### Identifiers

- **PUT** isteği ile index ekleyebileceğimizi biliyoruz. Eğer seçtiğimiz ID varolan bir indexi işaret ediyorsa ve biz böyle bir denk gelme durumunda uyarı versin istiyorsak, ```[PUT]: products/_create/1``` kullanabiliriz. Yaptığımız tek farklılık ```_doc``` yerine ```_create``` kullandık.
- Eğer ID otomatik verilsin istiyorsak, **POST** isteğinde bulunabiliriz; ```[POST]: products/_doc```
- Best Practice için ID'yi client olarak kendimiz vermemiz önemlidir.

### Indexing

- _refresh_interval_ field'ı, bize bir data eklendikten ne kadar süre sonra sorgulanabileceğini belirtir. Default olarak bu süre 1 saniyedir. Örneğin;
```
PUT products/_settings
{
	"index": {"refresh_interval_": "5s"}
}

=> 

{
	"acknowledged": true
}
```


- *refresh* özelliği ise default olarak false olup, PUT isteklerinde true olarak ya da wait_for olarak ayarlanabilir.
```
PUT products/_doc/20?({refresh=false, refresh=true, wait_for} seçilebilir. wait_for seçilirse, settings'te belirlemiş olduğumuz süre sonrasında sorgulama yapılabilir. refresh true ise, anında sorgulama yapılabilir.)
{
	"name": "iPhone 6",
	"rating": 100,
	"published": true,
	"category": "mobile phone"
	"price": {
	   "usd": 3000,
	   "eur": 2500
	}
}
```

### Updating Document

- [POST]: <index_name>/_update/id isteği ile doküman güncelleme işlemi yapabiliriz. Seçili ID'de bir data yok ise hata fırlatır.
```
POST products/_update/1
{
	"doc": {
		"name": "Samsung X",
		"rating": 100,
		"published": true,
		"category": "mobile phone"
	}
}

=> 

{
	"acknowledged": true
}
```

