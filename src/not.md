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

---

## Data Types

- Common Types
	- text: string messages	
	- boolean: true/false
	- date: ISO 8601 => yyyy-MM-dd
	- numeric: byte/short/integer/long + float/double + half_float/half_double
	- binary	
	- keyword: structured text, must not be broken down or analyzed => email, phone, SGK no

- Complex and Relational Types
	- object: JSON object
	- nested: class mantığıyla çalışan bir type'dır. user.name, user.surname diyerek field erişimi yapılabilir
	- flattened: tüm alt field'lar birer **keyword**'dür.
	- join: parent-child mantığı işlemektedir. document'ları eşleştirerek bu bağlantıyı kurarız
	
- Structured Types
	- geo_shape
	- geo_point
	- ip
	- date_range: 05/05/2001-07/07/2022	
	- ip_range
	 
---

## Mapping

- First Indexing
	1. Yeni bir Index oluştur.
	1. Yeni bir Schema oluştur.
	1. Document'i kalıcı olarak kaydet.
	
	(Daha sonraki işlemlerde ilk 2 adım gerçekleşmez.)

- Veri karmaşası yaratmamak adına, schema'yı bizim net olarak belirlememiz Best Practice bir davranıştır.

### Dynamic Mapping

```
PUT kalemler/_doc/1
{
  "name": "kalem 1",
  "price": 22.22,
  "created_date": "2010/01/01"
}

GET kalemler/_mapping

=>

{
  "kalemler": {
    "mappings": {
      "properties": {
        "created_date": {
          "type": "date",
          "format": "yyyy/MM/dd HH:mm:ss||yyyy/MM/dd||epoch_millis"
        },
        "name": {
          "type": "text",
          "fields": {
            "keyword": {
              "type": "keyword",
              "ignore_above": 256
            }
          }
        },
        "price": {
          "type": "float"
        }
      }
    }
  }
}
```

### Explicit Mapping

```
PUT products
{
  "mappings": {
    "properties": {
      "name": {"type": "text"},
      "price": {"type": "long"},
      "stock_no": {"type": "keyword"},
      "warehouse": {
        "properties":{
          "germany": {"type": "integer"},
          "turkey": {"type": "integer"}
        }
      }
    }
  }
}

GET products/_mapping

=>

{
  "products": {
    "mappings": {
      "properties": {
        "name": {
          "type": "text"
        },
        "price": {
          "type": "long"
        },
        "stock_no": {
          "type": "keyword"
        },
        "warehouse": {
          "properties": {
            "germany": {
              "type": "integer"
            },
            "turkey": {
              "type": "integer"
            }
          }
        }
      }
    }
  }
}
```

- Var olan field'a alt field ekleyebiliriz, lakin mapping'te bu field'ın type'ını değiştiremeyiz.

- Bu senaryoyu gerçekleştirmenin tek yolu, re-index işlemidir. Bunu sağlamak için ise _reindex endpoint'ine **POST** isteği göndererek eski indeximizdeki verileri yeni index'e kopyalarız

```
POST /_reindex
{
  "source": {
    "index": "products"
  },
  "dest": {
    "index": "newProducts"
  }
}
```	 

---

## Searching

### Çalışma Mekanizması

1. Seçtiğimiz shard sayısına göre tüm index, ayrı ayrı **Node**'lara alınır. Biri koordinatör olmak üzere, shard sayısı kadar ayrılırlar ve her **Node**, bir shard tutar. (Koordinatör zaman içerisinde değişebilir)
1. Arama isteği geldiği zaman koordinatör Node, geri kalan tüm Node'lara **eşzamanlı olarak** istek gönderir. Bu istek, aynı index'e gönderilir yani alakasız bir index'e değil. Koordinatör, nereye istek göndereceğini bilen bir yapıdır.
1. İstek sonucu, response olarak döner.

### Rest API

- _search endpoint'ine istek göndererek arama yapabiliriz.
- GET/POST olarak isteğimizi gönderebiliriz. Arama kriterlerini query-string olarak yahut request body olarak betimleyebiliriz.

```
GET kibana_sample_data_ecommerce/_search?q=user:berkay

GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "match": {
      "user": "berkay"
    }
  }
}
```

### Search Context

- Query Context => skor değeri elde ettiğimiz arama yöntemi. örneğin: text arama
- Filter Context => skor hesaplaması yapılmaz. elasticsearch'tan gelen datanın ne kadar ilişkili olduğu ile ilgili bir derdimiz yok ise bu yöntemi kullanıp, ayrıca **caching** de uygulayabiliriz. örneğin: datetime arama

### Search Response

```
{
	"took": 1, (execution time - milisecond)
	"timed_out": false,
	"_shards": {
		"total": 1,
		"successful": 1,
		"skipped": 0,
		"failed": 0
	},
	"hits": {
		"total": {
			"value": 5, (toplam result)
			"relation": "eq"
		},
		"max_score": 1, (en yüksek skor)
		"hits": [
			{...},
			{...},
			{...},
			{...},
			{...} (skora göre sıralanır)
		]
	}
}
```

---

## Term-Level Queries

### Tam Eşleşme
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "term": {
      "customer_first_name.keyword": "youssef"
    }
  }
}
```

### Çoklu Eşleşme
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "term": {
      "customer_first_name.keyword": ["youssef", "jim"]
    }
  }
}
```

### Prefix Eşleşme
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "prefix": {
      "customer_first_name.keyword": "yous"
    }
  }
}
```

### ID'ye Göre Eşleşme
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "ids": {
      "values": ["1", "2", "3"]
    }
  }
}
```

### Field'ın Olup Olmadığını Kontrol Etme
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "exists": {
      "field": "order_id"
    }
  }
}
```

### Range Query
``` 
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "range": {
      "taxless_total_price":{
        "gte": 50,
        "lte": 100
      }
    }
  }
}
```

### Wildcard Query
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "wildcard": {
      "customer_first_name.keyword": "yous*"
    }
  }
}

GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "wildcard": {
      "customer_first_name.keyword": "*sef"
    }
  }
}

GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "wildcard": {
      "customer_first_name.keyword": "yous?ef"
    }
  }
}
```

### Pagination Query
```
GET kibana_sample_data_ecommerce/_search
{
  "size": 30, (kaç kayıt gelecek)
  "from": 7, (kaçıncı kayıttan itibaren almaya başlayacak)
  "query": {
    "multi_match": {
      "query": "Pyramidustries",
      "fields": ["manufacturer"]
    }
  }
}
```

### Sort Query
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "range": {
      "taxless_total_price":{
        "gte": 50,
        "lte": 100
      }
    }
  },
  "sort": [
    {
      "taxless_total_price": {
        "order": "desc
      }
    }
  ]
}
```

---

## Full-Text Queries

### Match
- Tokenized olarak en az bir eşleşme sağlanan kayıtları getir.
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "match": {
      "customer_full_name": "Yahya Goodwin" (tokenized olarak en az bir eşleşme sağlanan kayıtları getir)
    }
  }
}
```

- AND kullanılırsa tokenized olarak tam eşleşme sağlanan kayıtları getirir.
- Sıralamaya bakmaz! sadece o kelimeler var mı ona bakar
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "match": {
      "customer_full_name":{ 
        "query": "Yahya Goodwin", 
        "operator": "and"
      }
    }
  }
}
```

### Multi-Match
- Seçili field'lar için, hepsinde arama yapar
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "multi_match": {
      "query": "Robert Banks",
      "fields": ["customer_first_name", "customer_last_name", "customer_full_name"]
	  
    }
  }
}
```

### Match Bool Prefix
- Abdulraheem or Al or Shaw..
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "match_bool_prefix": {
      "customer_full_name": "Abdulraheem Al Shaw"
    }
  }
}
```

### Match Phrase
- Ancak ve ancak kelimelerin sırası öbek halinde uygunsa getirir
```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "match_phrase": {
      "customer_full_name": "Abdulraheem Al Shaw"
    }
  }
}
```

### Match Phrase Prefix
- "Ahmet Mehmet Yıl" ifadesini aratıyorsak şu ifadeleri getirir;
	- Ahmet Mehmet Yılmaz
	- Ahmet Mehmet Yıl
	- Ömer Ahmet Mehmet Yıldız

```
GET kibana_sample_data_ecommerce/_search
{
  "query": {
    "match_phrase_prefix": {
      "customer_full_name": "Abdulraheem Al Shaw"
    }
  }
}
```

---

## Compound Queries

- must = Kısıt mutlaka var olmalı. Bu kısıtlar skora etki eder
- filter = Kısıt mutlaka var olmalı. Bu kısıtlar skora etki **etmez**
- should = Kısıt var olabilir, OR davranışı gibi
- must_not = Kısıt mutlaka var olmamalı

---

## Aggregation Types

### Bucket Aggregations

#### Terms
```
GET kibana_sample_data_ecommerce/_search
{
  "_source": false, 
  "aggs": {
    "category_names": {
      "terms": {
        "field": "geoip.continent_name"
      }
    }
  }
}

=>

{
  "took": 4,
  "timed_out": false,
  "_shards": {
    "total": 1,
    "successful": 1,
    "skipped": 0,
    "failed": 0
  },
  "hits": {
    "total": {
      "value": 4675,
      "relation": "eq"
    },
    "max_score": 1,
    "hits": [
	...
    ]
  },
  "aggregations": {
    "category_names": {
      "doc_count_error_upper_bound": 0,
      "sum_other_doc_count": 0,
      "buckets": [
        {
          "key": "Asia",
          "doc_count": 1220
        },
        {
          "key": "North America",
          "doc_count": 1206
        },
        {
          "key": "Europe",
          "doc_count": 1172
        },
        {
          "key": "Africa",
          "doc_count": 899
        },
        {
          "key": "South America",
          "doc_count": 178
        }
      ]
    }
  }
}
```

#### Ranges
```
GET kibana_sample_data_ecommerce/_search
{
  "aggs": {
    "category_price_range": {
      "range": {
        "field": "taxful_total_price",
        "ranges": [
          {"to": 10.00},
          {"from": 10.00, "to": 100.00},
          {"from": 100}
        ]
      }
    }
  }
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
      "value": 4675,
      "relation": "eq"
    },
    "max_score": 1,
    "hits": [
      ...
    ]
  },
  "aggregations": {
    "category_price_range": {
      "buckets": [
        {
          "key": "*-10.0",
          "to": 10,
          "doc_count": 3
        },
        {
          "key": "10.0-100.0",
          "from": 10,
          "to": 100,
          "doc_count": 3666
        },
        {
          "key": "100.0-*",
          "from": 100,
          "doc_count": 1006
        }
      ]
    }
  }
}
```

### Metric Aggregations

#### Avg

```
GET kibana_sample_data_ecommerce/_search
{
  "_source": false, 
  "aggs": {
    "avg_price": {
      "avg": {
        "field": "taxful_total_price"
      }
    }
  }
}

=>

toplam ortalamayı getirir (AVG, GROUP BY mantığı)
```

#### Sum/Max/Min

```
GET kibana_sample_data_ecommerce/_search
{
  "_source": false, 
  "aggs": {
    "avg_price": {
      "sum/max/min": {
        "field": "taxful_total_price"
      }
    }
  }
}
```