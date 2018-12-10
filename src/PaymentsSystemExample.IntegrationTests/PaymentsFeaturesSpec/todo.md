// TODO: add check of payment and real payment builder with mocked data.
// TODO:  All the scenarios to cover
//  Posting not handled HTTP Methdo -> correct error
//  Posting to non existing endpoint -> 404

//  scenarios:
//  get (done)
//   - 200  (done)
//   - 404 -> no payment id (done)

//  put (full update replace)
//   - 200 
//   - incorrect data - validation error
//      - missing values
//      - incorrect amount
//      - incorrect date format
//  patch (partial update)
//    - payment id -> 
//  list -> get on resource with S
//  delete ->
//        404 -> payment
//        200 -> success
//  all  -> mocked basic auth token
//   - 403 -> no token
//   - 403 -> token - org mismatch
//   - error -> no org id no version and other metadata
//   - 500 -> how to test it? exception handling within services

// Updates need to check if version number has not changed
// Optimistic concurrency
// -> return -> 412 item modified
///  https://stackoverflow.com/questions/5369480/when-is-it-appropriate-to-respond-with-a-http-412-error 
