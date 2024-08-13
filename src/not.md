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

- Seçili ID'de bir document yoksa, 404 döndürür.

```
{
	"_index": "products",
	"_id": "1",
	"found": false
}
```



### Retrieving Documents with HEAD method

- Eğer seçtiğimiz ID'de bir document bulunuyorsa, sadece var olup olmama bilgisinin incelenmesi adına bu metot tipinden faydalanabiliyoruz. Bu sayede response'da döndürülen fazla bilgilerden kurtuluyoruz.

```
HEAD products/_doc/1

=> 

200 - OK

ya da

{
	"statusCode": 404,
	"error": "Not Found",
	"message": "404 - Not Found",
}
```



### Retrieving Multiple Documents

- query field'ı yardımıyla getirme

```
GET products/_search
{
	"query": {"match_all": {}}
}

=>

{
	"took": 1,
	"timed_out": false,
	"_shards": {
		"total": 1,
		"successful": 1,
		"skipped": 0,
		"failed": 0
	},
	"hits": {
		"total": {
			"value": 5,
			"relation": "eq"
		},
		"max_score": 1,
		"hits": [
			{...},
			{...},
			{...},
			{...},
			{...} (documents with score field)
		]
	}
}
```

- Eşleşen ID ile getirme

```
GET products/_mget
{
	"ids": [2,3]
}

=>

{
	"docs": [
		{...},
		{...} (documents)
	]
}
```

- Eşleşen index ve ID ile getirme

```
GET products/_mget
{
	"docs": [
		{
			"_index": "products",
			"_id": 2,
		},
		{
			"_index": "products",
			"_id": 3,
		}
	]
}

=>

{
	"docs": [
		{...},
		{...} (documents)
	]
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
	"_index": "products",
	"_type_": "_doc",
	"_id": "1",
	"_version": 3,
	"result": "updated",
	"_shards": {
	   "total": 2,
	   "successful": 1,
	   "failed": 0
	},
	"_seq_no": 7,
	"_primary_term": 1
}
```



### Deleting Document

- [POST]: <index_name>/_doc//id isteği ile doküman silme işlemi yapabiliriz. Seçili ID'de bir data yok ise 404 döndürür. result: not_found olur.
```
DELETE products/_doc/5

=> 

{
	"_index": "products",
	"_type_": "_doc",
	"_id": "5",
	"_version": 22,
	"result": "deleted",
	"_shards": {
	   "total": 2,
	   "successful": 1,
	   "failed": 0
	},
	"_seq_no": 37,
	"_primary_term": 1
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
- refresh=false, refresh=true, wait_for} seçilebilir. wait_for seçilirse, settings'te belirlemiş olduğumuz süre sonrasında sorgulama yapılabilir. refresh true ise, anında sorgulama yapılabilir.
```
PUT products/_doc/20?{refreshType}
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