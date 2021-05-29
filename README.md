# Cryptography_DigitalSignatureByRSA_IvanovA
Realization of Digital Signature on RSA algorithm by IvanovA (2021).

- How to use it? 

This algorithm is able to work in a short time with numbers up to 2^1024 (10^309). It is recommended to set the number of checks (Acc) equal to the square root of the specified number (For 10^309 - 1024). This is necessary for the correct operation of the simplicity test, which is made according to the Miller-Rabin algorithm. 
For it's work just follow the necessary steps indicated on the buttons (Number). After pressing the button '4) Get signature' you may write some text (which you need to send) into the textbox.
