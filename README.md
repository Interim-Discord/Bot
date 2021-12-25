# Interim Bot
## Functionality
- Timezone roles.
- Thread started message deletion (deleting the "X started a thread" message when it's directly after the start post).


## Usage
### Token
Token is passed as a program argument "-token XXXXXX".  

### Commands
#### !setup
- log  
  Sets *this* channel as the channel to send bot logs.  
- time zone assignment  
  Creates the local times assignment message in *this* channel.  
- thread started deletion **on/off**  
  Enables/disables the thread started message deletion feature (disabled by default).  
- time zone colours **on/off**  
  Enables/disables colours for the time zone roles (disabled by default).  
#### !log
- ping  
- time zone roles  
  Logs the saved data linking time zones to role IDs.  
- preferences  
  Logs the saved preference data.  
#### !interim
- tear down data  
  Removes all role data from the server for this guild.  
- tear down roles  
  Removes all roles that look like times. Regex is: `^[0-9]{2}:[0-9]{2} [AP]M$`.  
