

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

## når man typer password, i box skal den også udregner reglerne med

## if users gets derank from admin to none ,,, (don't ask users to enter password, remove it from user)

## if users has allrede set password don't show password popup..

## Lås endpoints som /users-dashboard, /admin,  (registrations) kræver admin users profile

## i admin login fjern så man ikke behøver at logge in med telefon nummmert

## record updateret 

## password skal kunne updaters i en post request , for nye admins osv
## repo skal fixeds
## service skal fixeds
## controller måske
## i frontend hvis specialle filer til kald af apier skal updateret
## typescript skal updatet så et password også bliver inkludetet

## if users gets updated to admin.......... password is not sete

## remove taps som this week, this month, this year, 

## hvis telefon nummeeret ikke findes lav text rød

## password bruges til at indentificere brugeren gør så .......... username kan gører det istedet ............

## update error message from (" der opstod en fejl . Prøv igen til ....... navnet existere, telefon existere,... osv)

## tilføj så hvis man checker ud fortidligt vises det i checkin/checkout htmlen du mangler tid 

## update /checkin, checkout, i dashboard er der time on off, af pr registration...

## not possible to have two users name TEST , with same phone number but diffent countrycode
## should be possible that two users can have the same names'

## improve delete modal ændrer cancel og delete rækkefølgen + når modal er åben lav delete knappen rød fra starten
-----------------------------------------------------------------------------------------------------------
# LIST DOES NOT MATTER /// FIX LIST 

## change all spots their is something on danish to english. (does not matter)

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

## edit not possible in registartion...

# i password change er der to øjenen 

## admin max min på username 
-----------------------------------------------------------------------------------------------------------
## Importans fixes

## small improvements...

##  make registration tap under each users
## so instead of all appear like one after one , drodropdown if  a users make more han one

## fjerne fra all users dashboard men der er admins / mulighed skal det virklig være der????

## can edit more than one users at each time 
## to edit phone number, countrycode requires password (otp)
## edit so ikke en hel colune for countrycode men det står i phone.....

## lav et password mere sikket???? send code 1-6 altid ikke særlig sikkert


## opret en registration hver gang en admin logget ind, (hvis lyst og checkud når token udløber)...
## adminds vil ikke skulle checket ud på bestemt tidspunkt


-------------------------------------------------------------------------------------------------------------------
# ITVIL
## lave så man ikke skal bruge telefon nummer for at checke ind men  sit cpr nummer (svært kan ikke encypt det) cpr må ikke vises i dashboard...

## checkin problem .... når flere bruger kan havde det samme telefon nummert men forskellig landcode
## hvordan ved man så hvilken en der checker in????............... gør så telefon skal være unik.

## byg system så hvis man checker ud for tidligt kan tykke på vent.... eller checkout alligvel,,,
## skal også vises hvor mange timer man skal vente ... for det glæder (ikke nødvendigt fordi når folk checker ud vil de jo ud....)
-------------------------------------------------------------------------------------------------------------------
## code errors

## dashboard som viser hvis en users checket in i dag, men ikkke i går.... byg et  skole skema som viser statekstiken... (smart men problem ville være nok et skema pr bruger, og hvis 100 bruger how?)
# bedre nok at lave et skema som kan regne fravær ud...... ( tager ... om de er forsent og udregner tal)
## kan vises under user.. og fra i % ....(i alt).......

## checkout skal være ok hvis man checker ud på minut tillagt, / fjern så det ikke skal være perfekt i sekunder
-------------------------------------------------------------------------------------------------------------------
## design errors

## og hvis man checker ud før lav texten rød.... eller gul (problemer svær opgave)

## update /checkin, checkout, i dashboard er der time on off, af pr registration...(i login.html)
----------------------------------------------------------------------------------------------------------------



