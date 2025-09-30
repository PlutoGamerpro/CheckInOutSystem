


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

### Example:
```csharp
if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
    throw new UnauthorizedAccessException("Invalid password");
```

### Password rules

####  1 Minimum one uppercase letter and one lowercase letter

#### 2 At least one number

#### 3 At least one symbol (e.g., ¤=%?#%#@")

#### 4 Password length: minimum 8 (maybe change to 6)

#### Example: Run!1000



## not fixed 





### Create methods for the items below (to avoid repeating code)
##### implement json token for better sercuity  

### OPEN QUESTIONS

#### Is this necessary? Admin is already the highest role, and there aren’t many admins.

#### Should roles higher than Admin be able to delete Admins (and roles below Admin+)?


