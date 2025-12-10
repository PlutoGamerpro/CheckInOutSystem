

## ADMIN
## User for testing
## phone: number: 99999991
## password %Rest!10KE


##### FIXED LIST

## do i need a manger role??? or is it not needed???
## fjern evt manager helt fra programmet 

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

## can not deletet user error because your is foregin / primary key probaly the problem

## not fixed/implementet

## import can, delete user without confirmation, / registartion

## opdaret login,signup,admin forms ...........

## implement json token for better sercuity  

## nav bar 

## ændreder order på admin login så password til sidste

## drop dropwn med edit ,,, users country 

## admins brude nok ikke appear in dashboard kan ikke slettes 

## error admin can't change user name,phonenumber, countrycode, (derank, delete not possible.... only throw secret code)

## check on time / late

## idee istedet for alle skal checke in kl 8.00 kan der checkes in 8.10 og må gå 15.10 istedet for 10..
## tifløjet hvis man checker ud for tidligt så tilaføj noget der ændrer tekstra fra sluttidspunkt til checkout too early and specify the number how many hours / minuts to early........fx checkout 1 hour before allowedd

## make show all minutses and hours late not only minutes........

## for at kunne derank anden admin opdagere users til admin kræves kode
## code virkere kun en gang hvis flere gange virkere den ikke!!

## not crazed system ... fordi nedefor brude ikke være en fejl

## Perfect! I found the problem. The TokenService only adds the "Admin" role claim if user.IsAdmin is true. When ## you're logged in as a regular user (not admin), the token doesn't have the "Admin" role, so when you try to save, ## the [AdminAuthorize] filter rejects it with 403 Forbidden.

## The endpoint is marked with [AdminAuthorize] - which requires the token to have "Admin" role
## But a regular user cannot call this endpoint even to update their own name/phone
## The user likely logged in as a non-admin, so their token doesn't have the "Admin" role claim

## calendard does not work more (erro 400 badrequest, failed loading calendar month)

## add extra secret code to demote admins if knows can derank admin / update other users to admin!


## Lås endpoints som /users-dashboard, /admin,  (registrations) kræver admin users profile
-----------------------------------------------------------------------------------------------------------

# LIST DOES NOT MATTER /// FIX LIST 

## change all sports their is something on danish to english. (does not matter)

## some letter like x probaly other can't be typed in (Fornavn, Efternavn) (does not matter)

## role removed.
## make an manager only manager and also only manager (still errors not maded)
  ### then try to run api calls where admin only can , and do the same for the manager and test .. 
  ### add so a admin can't delete a other admin but only a manager
  ### add so a manager can't delete a other manager but only a users. 
  ### add so a admin can't delete a other admin but only a manager
  ### add so a manager can't delete a other manager but only a users. 

## hardcoded-credentials Embedding credentials in source code risks unauthorized access (app settings) 
## Create methods for the items below (to avoid repeating code)------- would have best practice to use other service


## token er brugt i seperat og i api kald filen (blandet ikke godt et sted bedre)

## maybe opdate ( so only usersname and password to login)

## FIX create profil ui problmes

## should add some phone number policy so not just randoms number likes +4444444

## plus language emoj

## fx gamle branch feature branch for importns file,,, and that why you put it in github

## maybe add option to edit an registration but , best not so no one can maniplute the system... 

## could add a note to every registration possible to add one ,,, checkout a reason....

## update user dashboard..........(id nummer range profilerne)
-----------------------------------------------------------------------------------------------------------
## Importans fixes

## small improvements...

##  make registration tap under each users
## so instead of all appear like one after one , drodropdown if  a users make more han one

## fjerne fra all users dashboard men der er admins / mulighed skal det virklig være der????

## can edit more than one users at each time 
## to edit phone number, countrycode requires password (otp)
## edit so ikke en hel colune for countrycode men det står i phone.....

## code errors

## edit not possible in registartion...


## design errors

## hvis telefon nummeeret ikke findes lav text rød
## og hvis man checker ud før lav texten rød.... eller gul 

## improve delete modal ændrer cancel og delete rækkefølgen + når modal er åben lav delete knappen rød fra starten


## 
