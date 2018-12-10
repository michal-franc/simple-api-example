Table of Contents
=================

  * [Summary:](#summary)
  * [Required to Run:](#required-to-run)
    * [Optional](#optional)
  * [How to run?](#how-to-run)
  * [How to stop and cleanup?](#how-to-stop-and-cleanup)
  * [How to run tests?](#how-to-run-tests)
  * [How to populate DynamoDB?](#how-to-populate-dynamodb)
  * [How I approached this task?](#how-i-approached-this-task)
  * [.NET open source packages used in the project:](#net-open-source-packages-used-in-the-project)
  * [Why I picked up DynamoDB?](#why-i-picked-up-dynamodb)

*Created by [gh-md-toc](https://github.com/ekalinin/github-markdown-toc)*

### Summary

!["basic diagram"](/diagrams/basic_diagram.png)

Payments system API sample using [Form3 API](http://api-docs.form3.tech/) as domain example. Approach with one single API.
- code was written on Linux Mint using I3wm and vscode + cli scripts

### Required to Run
- [.net code sdk 2.1](https://dotnet.microsoft.com/download)
- [aws cli](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html)
  - to create dynamodb on localstack
- [docker](https://docs.docker.com/install/linux/docker-ce/binaries/#prerequisites) 
  - Docker Engine 17.12 CE (tested on 17.12.0-ce, build c97c6d6)
  - if you have older or newer version you will need to change **docker-compose.yml** file version based on [link](https://docs.docker.com/compose/compose-file/#compose-and-docker-compatibility-matrix)
##### Optional
- [entr](http://eradman.com/entrproject/), [ripgrep](https://github.com/BurntSushi/ripgrep)
  - to run make auto-test 
  - it runs unit tests whenever a file is changed

### How to run?

Go to main folder and
```
make start
```
This command:
- starts docker composed [localstack](https://github.com/localstack/localstack) in detached mode with stubbed **DynamoDB**.
- runs a script to create **payments** table on localstack DynamoDB
- starts dotnet application on port **5000** for http and **5001** for https

### How to stop and cleanup?
Go to main folder and
```
make stop
```
This command:
- removes localstack and cleans up images

### How to run tests?
Go to main folder and
```
make all-tests
```

This command:
- runs unit tests
- starts up localstack with dynamodb and runs a script to create payments table
- runs integration tests
- stops localstack with dynamodb

### How to populate DynamoDB?
Go to main folder and
```
make put-single
```
For one payment.

```
make put-multi
```
For multiple payments.

### How I approached this task?
- Started with empty repo nothing special here added .gitignore.
- Added placeholder generate app from dotnet new.
- Added unit test folder.
- Wired up basis unit tests running (make auto-test).
- Wired up basic integration tests with **LightBDD**.
- Started with discovering problem space by working on **json** parsing. At the beginning I don't want to start with UI layer or DB layer. Domain, core is most important. By parsing json I was able to figure out how the data looks like, what kind of validations I need and how to design this system. This will lead to potential ideas which DB to pick or how to structure the API. This is also the moment wher communication beetwen Engineer <-> Domain Expert starts to happen.
- Added first API using InMemDB (List :)) simulating some data layer.
- Wired up IOC and wrote first unit tests for controller.
- Wrote controller flow logic with unit tests with some basic BDD style integration tests (still using InMemDb).
- Wired up first method GET using DynamoDB on localstack.
- Moved on with DELETE, PUT, POST methods and Payments List GET.
- Refactorization Clean Up.
- Adding more integration tests - fixing bugs along the way with unit tests to cover them.
- Creating simple implementation of Domain validation using FluentValidation.

### .NET open source packages used in the project:
- [LightBDD](https://github.com/LightBDD/LightBDD)
  - opinionated but interesting approach to write simple BDD styled tests.
  - this was my first time using this package and I will definitely get back to it.
  - generates nice reports in html <img width="200px" heigth="100px" src="/images/light_bdd.png" />
- [XUnit](https://github.com/xunit/xunit)
  - my standard go to **Unit Testing** framework.
  - It doesn't have more complex Assertions like **NUnit** but this can be solved by adding **FluentAssertions**
- [FluentAssertions](https://github.com/fluentassertins/fluentassertions)
  - changes the way assertions are written, forcing you to use syntax **object.Should().Be(something)**
- [Moq](https://github.com/moq/moq)
  - my go to **Mocking** framework. It just works :)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
  - Built in serialization provided in MVC.Core are ok but limiting.
- [FluentValidation](https://github.com/JeremySkinner/FluentValidation)
  - ASP.Net Core supports model binding and DataComponents attribute based validations, but using these can lead to MVC HTTP layer leaking out to Domain logic.
  - For me Domain is *sacred* and should be only about my business problem. UI, DB layers are kept away.
  - FluentValidation provides nice framework to create and wire up validations on your domain objects.

### Why I picked up DynamoDB?

- RDBSM is usually my default choice for start. It provides all the guarantess I need, plus can simplify application logic with transaction and rollback handling.
- At the moment requirements are quite simple - we just list, update, add and get 'objects'.
  - GET payment is simple and we can fulfill it based on querying either by PaymentId (one payment).
  - LIST payments is still simple and is based on OrganisationId - give me all payments for organisation
  - There are no requirements for any filter - like give me a payment with this amount or this beneficiary.
    - There is no requirement for patch.
    - So I don't need to fully denormalize the object and can keep 'attributes' potentialy as a JSON blob.
  - Update is also simple per Payment Id.
  - Version in json file suggest 'optimistic concurrency'.
  - What kind of indexes would I need? 
    - PaymentId 
    - OrganisationId
    - Maybe one sort key per processign date
- Not sure what kind of traffic i can expect -> write vs read? Assuming for this exercise that there will be more reads than writes -> as customer writes a payment once then polls (assuming no loopback with events or websockets) mutliple times to get the status. Ocassionaly user my build a more complex report by listing payments. Both assumptions suggest more reads than writes. Not sure how 'real time should be the system' and what kind of caching we need.
- Also how eventually consistent do we want the data to be? Do I need strong consistency? Or maybe I can relax some of the guarantees and for instace use 'Read your own Writes' approach to make an ilussion of strong consistency. For this task I will assume strong consistency and very simple system with one API and DB.
- Given these requirements plus I decided to either go with document or object based database. There is low benefit of normalized data. Given more complicated requirements like filtering of data based on criterias maybe then relational database would make the trick. But if searching is needed I would project the data to a different form - eg reporting database or used 'search optimised persistence stores' like ElasticSearch if full-text search would be a requirement.

#### RDBSM
 - mysql or sql 
    - supports proper transactions that would let me simplify application level and synchronization and just let rdbsm do the work
    - indexing support - I can create many different indexes + query optimization plans can help with the load - can be usefull if quesries are 'complicated'
    - I could use RDS or AURORA in AWS - both are managed solutions - providing clusters (Aurora might soon support Multi-Master setup).
      - encryption at rest support
    - at the moment i have no requirements for more complicated queries
    - if I dont use managed cluster or instance eg AWS (Aurora or RDS) I will have to deal with the support
      - plus sharding and replication - not sure if our team has experience with it
    - if mysql - which engine to pick? InnoDB is apparently recommended as the default one.
 
#### DynamoDB
 - fully managed
 - speed of development
 - simplicity with hidden complexity (you have to be carefull)
 - I have a lot of experience with it
 - fresh from Re-Invent - on demand dynamo! (can be risky but no need to fine tune read write capacity!! yey)
 - it magically 'works' and scales if you do you home work with proper partitioning
 - column wide schema less -> can be usefull but in this case maybe not the thing I want (it can also be scary)
 - I can extract most importants columns like PaymentId, OrganisationId, maybe ProcessingDate and keep the rest as a json blob
 - no need to wory about data replication, sharding etc
 - encryption at rest ( for aws centric company perfect choice with kms )
 - limitations on the 'data' footprint (one column value cannot reach certain amount of data)
 - partitioning strategy is really important as we want to avoid 'hot' nodes scenario that is making life difficult
 - ndexing is 'limited' - but for this scenario good enough
 - API is 'complicated'
 - costs 'can' get out of hand

#### MongoDB
 - it is no longer 'web scale' after last jepsen tests - it is a proper stable 'persistence store'.
 - payment as a 'thing' fits perfectly to 'document' based approach
 - atlas! - managed mongo looks really interesting
 - transactions (atomic operations on document but recently they added multi document transactions)
 - is Atlas scalable? hassle free enough?
 - no experience apart from simple hobby use cases years ago

### Other Notes
- HTTP for me is just a 'layer' that exposes my domain. So I started by thinking about my domain object - Payment.
- Usually I create classess and test code in the same file later moving them around to proper projects. That gives me fast feedback loop without context switching and thinking how to leverage file system to describe my code. I first experiment with my problem space and domain - discovering it before making decision how to properly encapsulate everything so that it makes sense.
- When writing code I use auto test scripts (entr + ripgrep) to enable continous testin when file change (similar to [wallaby.js](https://wallabyjs.com/) or [NCrunch](https://www.ncrunch.net/)).
- For Payment type initialy my instict was to use Enum but then I realised that if I pick up a document or relational persistent store - it doesn't really matter if I safe couple of bytes for not storing string. 
- For processing date I was a bit surprised why the date doesn't have any time zone specific information. Then I found that it is actually assuming format 'yyyy-MM-dd'.
- Initialy tests used versy simple ine memory database - a stub using 'List'. This speeds up initial development without context switching to DB layer.
- While building test scenarios for parser, I discovered that there are two ID's.
  - I made an assumption that Id of the metadata is the correct id to represent Id in our system and the one in the attributes is for 'external consumers id representaion and correlation'.
  - Ideally i would call the attributes 'ExternalPayment' and our main object InternalPayment - or figure out a different terminilogoy on how to differentatie beetwen them - int he form3 api reference i found that you fetch the payment using guid id not the 'integer' or stirng one 

### Things I would do with more time

- [ ] add license file
- [ ] limit list payments to 10 records
  - [ ] paging
  - [ ] filters
- [ ] compression and caching of requests
- [ ] delete - get - should require organisationId in the request to prevent users from scrapping data of other orgs or removing data
  - [ ] Security tokens and simple authorization - organisation id in the token vs orgnisation id used in the queries to data store
- [ ] handling creation of duplicate payments - and 409 code
- [ ] domain in separate project
  - [ ] json objects as separte classess in separete project
- [ ] more unit tests coverage
- [ ] more integration tests coverage
- [ ] testing in on the cloud (AWS)
  - [ ] running on k8s cluster
- [ ] optimistic concurrency when updating records
- [ ] load testing using k6 and my own simple example - https://github.com/michal-franc/docker-k6-influxdb-grafana
- [ ] Pass with more robust static code analysis tools like stylecop
- [ ] write tests to verify cache and compression
- [ ] throttling on the api 
  - ideally this should be on a different layer like api gateway of DDOS WAF protection
- [ ] metrics with prometheus
- [ ] influxDB + grafana for metrics
- [ ] logging to file
- [ ] High Availability - 3 instances - using docker and some load balancer on nginx 
  - [ ] route53 and ALB simulation on localstack (not sure if this is possible?)
- [ ] test if it builds on windows :)
- [ ] do a chaos engineering round of tests 
  - both the app and the unit tets (break code and see what kind of messages does the test give is it easy to understand?)
- [ ] Hateos like in the example file
- [ ] Content negotation
- [ ] Better isolation of data for organisations so that everything is not in single table
- [ ] Proper design strategy on partitioning key pick - should we use composite? what sort key? etc etc
- [ ] Swagger and API discoverability
- [ ] Delete using tombstone approach with removal eventually consitent to enable 'accidental' recovery
- [ ] Patch Verb support
- [ ] https

### System I would build given 'infinite' amount of time and resources


[Table of Contents](#table-of-contents)
