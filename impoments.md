


##### FIXED LIST

#### Delete users without confirmation (FIXED)

#### Two users with the same encrypted password (tested – no error found, both had different hashes)

#### Owner role removed, and Manager/Admin or another role added so that Admin is the highest role

### Phone numbers should not be encrypted (makes things unnecessarily complex).  
Instead, validate phone numbers with proper checks.  
Decrypting them in the dashboard should not be possible (dashboard is not a safe option).

#### Remove extra password – only one password should be required to log in to the admin dashboard.  
(Currently there are two, and the secret does not need to exist)

### Admin extra class with additional features
#### External class / Manager class

### Password rules

####  1 Minimum one uppercase letter and one lowercase letter

#### 2 At least one number

#### 3 At least one symbol (e.g., ¤=%?#%#@")

#### 4 Password length: minimum 8 (maybe change to 6)

#### Example: Run!1000


## not fixed/implementet


## change all sports their is something on danish to english. 

## some letter like x probaly other can't be typed in (Fornavn, Efternavn)

### Create methods for the items below (to avoid repeating code)
##### implement json token for better sercuity  
## hardcoded-credentials Embedding credentials in source code risks unauthorized access (app settings) 

## make an manager only manager and also only manager

## then try to run api calls where admin only can , and do the same for the manager and test .. 

## add so a admin can't delete a other admin but only a manager
## add so a manager can't delete a other manager but only a users. 
