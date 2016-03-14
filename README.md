


Yahoo Weather Api

When creating an App in Yahoo:

1. Select 'Web Application' as the application type.

2. Specify a domain or subdomain in Callback Domain e.g. http://www.mydomain.com 
Urls like http://www.mydomain.com/api/callback are not allowed (you will get the "Invalid domain name" error). 
You will have to specify the full callback url in your code.

3. API Permissions: You should select at least one. If you don't you will keep getting consumer_key_rejected when trying to get a request token.
