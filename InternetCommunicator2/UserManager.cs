using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InterComm.Helpers;
using System.Windows;
using System.Windows.Forms;

namespace InterComm
{
    public class UserManager
    {
        /// <summary>
        /// md5 checksum of the file
        /// </summary>
        private string fileHash = string.Empty;

        /// <summary>
        /// why this is so fancy, is that i wanted the users to be changeable at runtime
        /// i want to be able to add users to the SessionSettings.usersfile, and i want that user
        /// to be automatically authorized, no restart, nothing. it should just work!
        /// the logic i use here, is to calculate the checksum of the SessionSettings.usersfile
        /// and if the file has not changed (i.e. users were not added or removed) then just reuse the old list
        /// otherwise reload the list and send it back. nice, huh? :)
        /// </summary>
        private List<User> _userList = new List<User>();
        private List<User> UserList
        {
            get
            {
                if (!File.Exists(SessionSettings.usersfile))
                {
                    MessageBox.Show("Users file " + System.Reflection.Assembly.GetExecutingAssembly().Location + "\\" + SessionSettings.langaugesfile + " missing... No users will be allowed to interact with InterComm", "InterComm", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Logger.LogInformation("users file not found! ignoring the error. allowing 0 users!");
                    throw new FileNotFoundException("users file not found: " + SessionSettings.usersfile);
                }
                string newFileHash = CryptoShit.GetMD5HashFromFile(SessionSettings.usersfile);
                if (newFileHash != fileHash)
                {
                    //file has changes.. load the users again!
                    //save the current list of users! otherwise, their states will be forgotten!
                    //_userList.Clear();
                    List<User> oldList = new List<User>(_userList); //copy the old list, we need to maintain that info temporarily
                    _userList.Clear(); //we're gonna rebuild this list

                    fileHash = newFileHash;
                    string[] allowedUsersFromFile = null;
                    try
                    {
                        allowedUsersFromFile = File.ReadAllLines(SessionSettings.usersfile);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        throw ex;
                    }
                    foreach (string user in allowedUsersFromFile)
                    {
                        if (user.Length == 0) continue;
                        string[] names = user.Split(new string[] {",", "|", " "}, StringSplitOptions.RemoveEmptyEntries);
                        if (names.Length == 0)
                        {
                            Logger.LogInformation("users file is malformed. it should be in the format: <emailaddress><[|],[,],[ ]><nickname>");
                            continue;
                        }
                        string username = "";
                        username = names[0];
                        username = username.Trim();
                        username = username.ToLower();
                        //todo(3) rethink this logic. initially i just wanted the usernames
                        //but it grew to add nicknames, authlevels
                        if (username.Length > 0)
                        {
                            string nickname;
                            UserAuthLevel authLevel = UserAuthLevel.Regular;
                            //figure out nick name, 
                            //if no nick name has been mentioned, then assign username as nick name
                            if (names[1] == null) nickname = username; else { nickname = names[1]; nickname = nickname.Trim(); }
                            if (nickname.Length == 0) nickname = new EmailAddress(names[0]).Username;
                            if (names.Length==3)
                            {
                                try
                                {
                                    //parse the text as an enum
                                    authLevel = (UserAuthLevel)Enum.Parse(typeof(UserAuthLevel), names[2], true);
                                }
                                catch
                                {
                                    //since this is read from the file, i should just make it regular.
                                    authLevel = UserAuthLevel.Regular;
                                }
                            }
                            User newUser = new User(username, nickname, UserState.Allowed, authLevel);
                            _userList.Add(newUser);
                        }
                        else
                        {
                            HandleDataFormatError("username is empty, can not continue");
                        }
                    }//end foreach

                    // get the users back to the states they were in!
                    // for all users in oldList that are still in _usersList -> replace them in _userList
                    List<User> usersToRemove = new List<User>();
                    List<User> usersToReplace = new List<User>();
                    foreach (User u in oldList)
                    {
                        foreach (User n in _userList)
                        {
                            if (u.UserName == n.UserName) //is there a match?
                            {
                                //replace it!
                                //_userList.Remove(n);
                                usersToRemove.Add(n);
                                User userToReplace = new User(u);
                                usersToReplace.Add(userToReplace);
                                //_userList.Add(userToReplace);
                            }
                        }
                    }//end for
                    foreach (User del in usersToRemove) _userList.Remove(del);
                    _userList.AddRange(usersToReplace);
                }//end new file              
                return _userList;
            }
        }

        //add user function to add to users.ini
        public bool AddUser(string username, string nickname, UserAuthLevel auth)
        {
            bool result = false;

            Logger.LogInformation("runtime add user request for " + username + "(" + nickname + ") " + auth.ToString());
            UserState state = UserState.Allowed;
            User newUser = new User(username, nickname, state, auth); //remotely added
            try
            {
                //todo(0) fix the obvious bug here (append and dont introdude a new line)
                StreamWriter writer = File.AppendText(SessionSettings.usersfile);
                writer.WriteLine(); //extra!
                writer.WriteLine(username + "|" + nickname + "|" + auth.ToString());
                writer.Flush();
                writer.Close();
                result = true;
            }
            catch (Exception ex)
            {
                Logger.LogInformation("error adding user to usersfile");
                Logger.LogException(ex);
                result = false;
            }
            return result;
        }

        public bool RemoveUser(string username)
        {
            bool result = false;
            Logger.LogInformation("runtime remove user request for " + username);
            User remove = GetUser(username);
            //check if the user is valid
            if (remove.State == UserState.Invalid)
            {
                Logger.LogInformation("no user with username " + username + " exists in the userlist.");
                result = false;
                return result;
            }
            remove.State = UserState.Removed; //redundant
            remove.AuthLevel = UserAuthLevel.Unauthorized; //redundant
            try
            {
                StreamWriter writer = File.AppendText(SessionSettings.usersfile_temp);
                string[] curUsers = File.ReadAllLines(SessionSettings.usersfile);
                foreach (string user in curUsers)
                {
                    if (user.StartsWith(remove.UserName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //this user needs to be removed... skip him
                        continue;
                    }
                    writer.WriteLine(user);
                }
                writer.Flush();
                writer.Close();
                //replace original file with temp file
                File.Delete(SessionSettings.usersfile);
                File.Move(SessionSettings.usersfile_temp, SessionSettings.usersfile);
                result = true;
            }
            catch (Exception ex)
            {
                Logger.LogInformation("error removinng user from usersfile");
                Logger.LogException(ex);
                result = false;
            }
            return result;
        }

        //remove user function to remove from users.ini

        /// <summary>
        /// error handling, the users file may  be malformed
        /// </summary>
        /// <param name="errorInfo">error string</param>
        private void HandleDataFormatError(string errorInfo)
        {
            Logger.LogInformation(errorInfo);
            throw new InvalidDataException("users file " + SessionSettings.usersfile + " is malformed");
        }

        /// <summary>
        /// constructor - triggers loading the users file for the first time.
        /// the "log" below is important for the trigger. do not remove it.
        /// </summary>
        public UserManager()
        {
            Logger.LogInformation("found " + UserList.Count + " users that are enabled");
        }

        /// <summary>
        /// returns the internal User object corresponding to a user
        /// </summary>
        /// <param name="username">the email id of the user in question</param>
        /// <returns>the internal user object used to manage the user</returns>
        public User GetUser(string username)
        {
            foreach (User user in UserList)
            {
                if (username == user.UserName)
                {
                    //found a user that is allowed.
                    return user;
                }
            }
            User newUser = new User(username, "UnAuth", UserState.Invalid, UserAuthLevel.Unauthorized);
            return newUser;
        }
    }
}
