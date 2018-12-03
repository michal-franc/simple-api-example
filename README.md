----------------------------------------------------------
Design requirements:

• API should be RESTFUL

• API should be able to:
-- Fetch a payment resource
-- Create update and delete a payment resource 
-- List a collection of payment resource
-- Persist resource state (e.g. to a database)

-- One big integration test flow using Light BDD
  -- Create payment
  -- List Payment
  -- Update Payment
  -- Delete Payment
  -- List Payment

Example use case: As an example, a JavaScript client should be able to call the API and list a collection of payments (building a client is outside the scope of this exercise however).
Please submit your design as a PDF.
Implementation Implement the above design. Java is preferred, but if you are unfamiliar with Java, another object- oriented language such as C#, Ruby or Go may be used.
Implementation requirements:
• You should use best practice, for example TDD/BDD, with a focus on full-stack testing
• Prioritize correctness, robustness, and extensibility over extra features and optimizations.
• Write your code with the quality bar you would use for production code.
• Try to simplify your code by using open source frameworks and libraries where possible
----------------------------------------------------------


Payments system sample using Form3 API as domain example.

- TODO: Form3 API URL
- TODO: Form3 URL Log
- TODO: Runbook and analkysis of faults?

requirements:
- entr, ripgrep -> add to installation scripts
- .net sdk 2.1 something (exact version)

MakeFile: 
- what are the options available
- what is entr
- what is rg

Picked frameworks:
- LightBDD -> i have most expewrience with specflow but this one is cumbersone to use and requires extensions to visual studio - i am mostly vim, visual studio code user so i had to find alternative and i found multiple frameworks -> picked up LightBDD (which was not easy to set up as their docs were lacking (but i pushed a PR with more information to their docs))
- Xunit


Plan:
- as a User of this API what I would like to receive and do?

API needs to provide:
- HTOES -> https://github.com/faniereynders/aspnetcore-hateoas
- Content negotation
- Security Tokens in header -> Fake Auth server? Option for dev machine -> and tests
- Swagger?

- Add Logging

LOAD TESTS! using k6

HTOES: -> lib in .NET

 -> Things that need to invalidate payment
   -> Incorrect Type
   -> version -> ? how to use it?
   -> organisation_id -> if not existing error

   -> validations
      -> amount -> . vs , -> throw error no auto conversion -> assume only .
      -> check if is guid -> check if exists
      -> validate if payload ID was used or not

 -> When I post payment with same ID again it clashes and returns error
   -> should i return message that payment already exists? or could this be a security hole?

 -> When i Post to endpoint that doesnt exit i get 404
 -> When i put a proper payment i get 200
 -> When i ask for paayment info and it doesnt exist i get 404
 -> When i ask for exisitng payment i get the data and 200
 -> When i delete payment that doesnt exist i get 404
 -> When i delete payment that does exist i get 200
 -> When i delete payment that was already deleted i get 404
 -> when i post (update) payment that doesnt exist i get 404
 -> When i post (update) that exits i get 200

 -> Integraiton test for processing date -> how to deal with timezones?

- as a User of this API what kind of errors I would like to see?
- as a User of this API what kind of 'faults' I could make? (looking at payload)

-> create behaviours out of this -> codify them in the BDD styled tests
-> First list in TODO comments in the BDD styles tests

As i approached doing this.
- HTTP for me is just a 'layer' that exposes my domain.
- So i started by thinking aobut my domain object - Payment.
- Usually i create classess and test code in the same repo and then moving them around to proper projects. That gives my fast feedback loop without yet thinking on file system representation of my code.
- I also use auto test scripts using entr that enables me to run unit tests quicklky when the file is changed (similar to wallaby.js or ncrunch)
- Provided json representation of domain for me is just a projection
- I will pick up some document based database or object based one as I dont see a huge benefit of data normalization in this case when the operations are soo simple and based on correltion id - payment id
- If searching is needed i would project the document database to different form - eg reporting database or used even 'search optimised persistence stores' like elastic search if that would be the requirement.
- This task is to provide api that is able to operate on 'Objects' per PaymentId
- For account tyupe and payment type initialy my instict was to use Enum but then i realised that if i picked up document or object persistent store - it doesn't really matter if i safe couple of bytes for not storing string. Also account type and payment type can be dynamic in the future and we might control what values are acceptable usiung different modularised logic instead of 'compiled type' typ safety check that would in the future require from us deploy of application to make changes on prod. Same with payment sub type and type
- For processing date i was a bit surprised why the date doesnt have any time zone specific informatin. assument that date in there is in UTC format and reflected this correctly in Domain.
- I created direct 'adapter' from http to domain. Didn't wanted to use reflection as it is slow and we need to optimize for write / read throughput.

- Started with Payment mapping as this was a great way for me to 'explore' how the payment object looks like and what kind of problems i can achieve. Created IPayment Mapper and first implementaiton for Json files (with this interface we are able to extend mapper to XML or orther formats)


