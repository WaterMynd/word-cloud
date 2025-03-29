# WordCloud

## Testing
A simple python API was developed to simulate the urls with texts.

## Possible changes:
- FileManager and WordManager can be singletons instead of being passed in parameters
- A Class Word could be created with the attributes: name, frequency and type


# Questions

## Concurrency Management:
### How would you handle multiple concurrent reservations that could potentially lead to race conditions?
Using a lock on the Channel Manager system entry endpoint for the bookings, this way only one booking was done at the same time.
A lock per month or ranges of dates could be used to optimize the process, if the dates were in more than one month lock those months.

### What strategies would you employ to ensure that two users do not end up booking the same room across multiple platforms at the same time?
The same lock system would work here too.

## Atomic Updates:

### How can we ensure that updates to room availability on all platforms (Hostel.com, Booking.com, Airbnb, etc.) are atomic and happen in a way that either all updates succeed, or none do?
A request sending the data could be sent where the system would make all the check to ensure it can be updated (without updating it) and responds with that information (if can update or not), based on all the responses a second request is sent with the order to update or cancel the action.
Question: In which case would this be needed? Isn't it better to update all systems possible and retry the ones that failed?

### What tools or technologies would you use to implement this, such as transactions, distributed locks, or other mechanisms?
Requests

## Error Handling and Rollback:
### If an error occurs during the room availability update process on any of the platforms, how would you design the system to roll back the changes made on the other platforms to maintain consistency?
A possible solution would be to keep track of the platforms transactions ids if possible and if a rollback is needed tell the platforms which transactions to rollback.

### What mechanisms would you implement to guarantee that the room inventory is restored to its correct state if an update fails midway?
After the update check if the update to the room was made, if not rollback all platforms

## External API Integration:
### What approach would you recommend for integrating with the external APIs of platforms (Hostel.com, Booking.com, Airbnb)?
Adapters could be implemented to have different ways of communications according to the different APIs of the platforms

### Should we rely on synchronous or asynchronous communication, and how would you handle network issues, timeouts, or failures in communication?
Across the multiple platforms, several communications will be made at the same time, to ensure a smooth experience for the user asynchronous communications should be used. Retry mechanisms should be implemented. If the number of communications are too much for the current server upgrading it could be a viable option.

### What would you add in order to make the system even more robust?
Encryption of sensible data in communications. Authentication system for all the platforms that can be based on the platform itself (so all of them have a different key to communicate)