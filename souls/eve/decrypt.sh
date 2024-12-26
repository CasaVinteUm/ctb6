#!/bin/bash

gpg --batch --passphrase-file passphrase.txt --import secret_0xA0571DCD62DF702A.asc
gpg --batch --passphrase-file passphrase.txt --decrypt message.asc
