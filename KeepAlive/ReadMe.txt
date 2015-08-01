This is a simple app to ping the website in order to prevent the worker process from being recycled.
The host, GoDaddy recycles their worker processes after five minutes of inactivity.  This means that
without pinging the site, everything has to be reloaded, making the site very slow.