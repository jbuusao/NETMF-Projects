BACKGROUND

This project started when I was building a web-enabled aquarium controller. One pre-requisite being a wireless connectivity between the aquarium controller and the Wi-Fi router, I started with the attached WiFly driver. Now this small project tests the WiFly driver via an application that turns a Netduino board onto a security Honey Pot: that is a web-application, exposed to the Internet, which emulates the (authentication screen of the) administration console of my Wi-FI router. This Honey Pot is meant to attract hackers trying to break into this pseudo-router, capturing at the same time the username & passwords in use.

Why this odd project? Because my aquarium is meant to be accessible via the Internet, so I wanted to assess the potential for attacks on the web server, and at the same time, test the driver in a fun way. The attached VS-solution contains both the driver and the honey pot, for a Netduino-Plus (I know, it already has an Ethernet plug, but I wanted to start easy before moving everything to my Netduino-Mini, which is the final target).

TO READ ON

http://forums.netduino.com/index.php?/topic/1088-webserver-on-netduinoplus-wired-or-wireless-library/page__p__7870__hl__%2Bhoney+%2Bpot__fromsearch__1#entry7870

