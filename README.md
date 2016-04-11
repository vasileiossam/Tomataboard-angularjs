Tomataboard was the muse of xxx in Ancient Greek Mythology.

Tomataboard board is a personal dashboard inspired by Momentum.
It is about focused.

It is mainly a playground to learn and expirement with the new ASP.NET Core.

--------------------------------------------------------------


Yahoo Weather Api

When creating an App in Yahoo:

1. Select 'Web Application' as the application type.

2. Specify a domain or subdomain in Callback Domain e.g. http://www.mydomain.com 
Urls like http://www.mydomain.com/api/callback are not allowed (you will get the "Invalid domain name" error). 
You will have to specify the full callback url in your code. 

3. API Permissions: You should select at least one. If you don't you will keep getting consumer_key_rejected when trying to get a request token.


How to make the Yahoo callback work in IIS Express with localhost

1. In the yahoo create app page specify a callback domain e.g. mydomain.com 

2. Add the entry below in your hosts file:
  127.0.0.1 mydomain.com 

3. Your oauth_callback should be mydomain.com/Yahoo/Callback

4. if http://127.0.0.1 doesn't work in your browser follow the instructions here:
https://cyanbyfuchsia.wordpress.com/2014/07/29/make-iis-express-works-with-http127-0-0-1/
