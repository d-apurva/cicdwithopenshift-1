package com.uat.base;




import java.awt.AWTException;
import java.awt.Robot;
import java.awt.event.KeyEvent;
import java.io.FileInputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Properties;
import java.util.concurrent.TimeUnit;

import org.apache.log4j.Logger;
import org.openqa.selenium.Alert;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.firefox.FirefoxDriver;
import org.openqa.selenium.ie.InternetExplorerDriver;
import org.openqa.selenium.support.events.EventFiringWebDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.Assert;

import com.uat.util.ErrorUtil;
import com.uat.util.TestUtil;
import com.uat.util.Xls_Reader;

public class TestBaseOld {
	
	
	public static Logger APP_LOGS=null;
	
	
	public static Properties CONFIG=null;
	public static Properties OR=null;
	public static Properties resourceFileConversion=null;
	
	public static Xls_Reader suiteXls=null;
	public static Xls_Reader TM_projectSuiteXls=null;


	private String userFile;
	public static Xls_Reader userXls =null;
	
	private static boolean isInitalized=false;
	private static boolean isBrowserOpened=false;	

	public static WebDriver driver =null;
	public static EventFiringWebDriver eventfiringdriver= null;
	public static WebDriverWait wait;
	WebDriverWait wait1;
	
	Calendar calendar;
	SimpleDateFormat Date;
	SimpleDateFormat Time;
	public static String screenshotDateTime;
	
	StackTraceElement[] stacktrace;
	StackTraceElement stackElement;
	String className;
	static int loginFailureCounter = 1;

	
	// initializing the Tests
	/**
	 * Initializes all the excel objects and properties files. Should be used at the very beginning of things.
	 * @throws Exception
	 */
	public void initialize() throws Exception
	{
		
		
		// logs
		if(!isInitalized)
		{			
			APP_LOGS = Logger.getLogger("devpinoyLogger");
			// config
			APP_LOGS.debug("Loading Property files");
			CONFIG = new Properties();
			FileInputStream ip = new FileInputStream(System.getProperty("user.dir")+"//src//com//uat//config/config.properties");
			CONFIG.load(ip);
			
			
			OR = new Properties();
			ip = new FileInputStream(System.getProperty("user.dir")+"//src//com//uat//config/OR.properties");
			OR.load(ip);
			
			
			resourceFileConversion=new Properties();
			ip=new FileInputStream(System.getProperty("user.dir")+"//src//com//uat//config/resourceFileConversion.properties");
			resourceFileConversion.load(ip);
			
			APP_LOGS.debug("Loaded Property files successfully");
			APP_LOGS.debug("Loading XLS Files");
	
			
			
			// xls file
			TM_projectSuiteXls = new Xls_Reader(System.getProperty("user.dir")+"\\src\\com\\uat\\xls\\TM_Project Suite.xlsx");
			
			suiteXls = new Xls_Reader(System.getProperty("user.dir")+"//src//com//uat/xls//Suite.xlsx");
			
			if (CONFIG.getProperty("environment").equals("Cloud")) 
				//userFile = 	"UAT Users_Cloud.xlsx";-rgensolutions domain
				userFile = 	"UAT Users_Click2Cloud.xlsx";//Click2Cloud domain		
			else
				userFile = "UAT Users_Local.xlsx";
			
			userXls=new Xls_Reader(System.getProperty("user.dir")+"//src//com//uat/xls//"+userFile);
			
			APP_LOGS.debug("Loaded XLS Files successfully");
			
			calendar = Calendar.getInstance();
			Date = new SimpleDateFormat("dd-MM-yyyy");
			Time = new SimpleDateFormat("hh.mm.ss");
			screenshotDateTime = "Images captured on "+Date.format(calendar.getTime())+" at "+Time.format(calendar.getTime());
			
			isInitalized=true;
		
		}
		
	
	}
	
	
	// open a browser if its not opened
	/**
	 * Opens the browser based on the type mentioned in the Config file and intializes explicit and implicit wait
	 * @throws Exception
	 */
	public void openBrowser() throws Exception
	{
		
		if(!isBrowserOpened)
		{
			if (CONFIG.getProperty("browserType").equals("IE"))
			{	
				 System.setProperty("webdriver.ie.driver", System.getProperty("user.dir")+"\\src\\com\\uat\\config\\IEDriverServer.exe");
				 driver = new InternetExplorerDriver();
			}    
			else if(CONFIG.getProperty("browserType").equals("MOZILLA"))
			{	
				driver = new FirefoxDriver();
			}
			else if (CONFIG.getProperty("browserType").equals("CHROME"))
				 driver = new ChromeDriver();
			
			isBrowserOpened=true;
			eventfiringdriver = new EventFiringWebDriver(driver);//Added By Swati Gangrade 30/10/2014
			eventfiringdriver.manage().window().maximize();
			String waitTime=CONFIG.getProperty("default_implicitWait");
			eventfiringdriver.manage().timeouts().implicitlyWait(Long.parseLong(waitTime), TimeUnit.SECONDS);
			wait = new WebDriverWait(eventfiringdriver, 120);
			wait1 = new WebDriverWait(eventfiringdriver, 10);
			  
			 
		}

	}
	
	// close browser
	public void closeBrowser()
	{
		if (isBrowserOpened) 
		{
			driver.quit();
			eventfiringdriver=null;
			driver=null;
			isBrowserOpened=false;
		}
		
	}
	
	//a utility to get package name of a class
	public String extractPackageName(String packName)
	{
		String actualPackageNameArray[] = packName.split("\\.");
		return actualPackageNameArray[actualPackageNameArray.length-1];
	}
		
	
	
	
	// Assert Strings
	/**
	 * Uses assert statement to compare strings. Used for Passing/failing a test case
	 * 
	 * @param expected
	 * @param actual
	 * @return true if strings match and false if dont
	 */
	public boolean compareStrings(String expected, String actual)
	{
		try
		{			
			Assert.assertEquals(actual , expected);
		}
		catch(Throwable t)
		{	
			ErrorUtil.addVerificationFailure(t);			
			APP_LOGS.debug("Strings do not match");
			return false;
		}
		
		return true;
	}
	
	
	
	
		//compare integers
	/**
	 * Uses assert statement to compare integers. Used for Passing/failing a test case
	 * 
	 * @param expected
	 * @param actual
	 * @return true if match and false if dont
	 */
		public boolean compareIntegers(int expected, int actual)
		{
			try
			{
				Assert.assertEquals(actual, expected);
			}
			catch(Throwable t)
			{
				ErrorUtil.addVerificationFailure(t);			
				APP_LOGS.debug("Values do not match");
				return false;
			}
			
			return true;
		}
		
		
		
		//Assert True
		/**		
		 *  Uses assert statement. Used for Passing/failing a test case
		 *  
		 * @param expected
		 * @param actual
		 * @return true if true and false if false
		 */
		public boolean assertTrue(boolean value)
		{
			
			try
			{
				Assert.assertTrue(value);
			}
			catch(Throwable t)
			{
				
				ErrorUtil.addVerificationFailure(t);			
				APP_LOGS.debug("Asserted False");
				return false;
			}
			
			return true;
			
		}
		
		/**
		 * This function opens the site defined in the config.properties file and logins based on role provided for which user is fetched from User Xls file.
		 * Note that the first user present in the Xls file, with the role passed in this function, is used to login.
		 * If the role is not present in Xls file or if proper role name is not passed to the function, then closeBrowser() function is called which quits the driver.
		 *  
		 * @param role		role based on User Xls file
		 * @return 			returns true if login is successfull and role matches on the site, otherwise returns false if role is not present in xls or if role doesn't match on the site
		 */
		public boolean login(String role){
			
			String actualSiteRoleIds= null;
			String username;
			String password;
			String xlsRole;
			
			APP_LOGS.debug("Trying login for role "+role); 
			
			try
			{
				if (!role.isEmpty()) 
				{
					
					
					for(int i =2 ; i<=userXls.getRowCount("Users") ; i++)
					{
						
						xlsRole=userXls.getCellData("Users", "Role", i);
						
						if(role.equalsIgnoreCase(xlsRole) )
						{
							APP_LOGS.debug("Role found in Users file. Fetching credentials");
							
							username = userXls.getCellData("Users", "Username", i);
							
							password = userXls.getCellData("Users", "Password", i);							
							
							//updating username based on domain from Config file for loging in
							username = getCompleteUsername(username);
														
							eventfiringdriver.get(CONFIG.getProperty("siteUrl"));									
							
							//this IF block verifies whether user's role mentioned in excel is actually same on site
							if (UATLoginWindow(username, password)) 
							{															
								//this For block waits to get user's security role IDs
								for (int i1 = 0; i1 < 25; i1++) 
							    {
							       if (actualSiteRoleIds==null || actualSiteRoleIds.equals("[]") || actualSiteRoleIds.equals("[ ]")) 
							       {
							    	   Thread.sleep(1000);
							    	   try
							    	   {
							    		   actualSiteRoleIds = eventfiringdriver.executeScript("return security.userType").toString();
							    	   }
							    	   catch(Throwable t)
							    	   {
							         
							    	   }
							       	}
							       	else
							       		break;       
							        
							    }
								
							
								APP_LOGS.debug("Comparing selected role and site role");
								
								//in case excel user has multiple roles mentioned in user xlsx file
								String[] multipleRoles = xlsRole.split("\\+");								
								
								//this For block verifies whether user's role mentioned in excel is actually same on site
								for (int j = 0; j < multipleRoles.length; j++) 
								{									
									if(!actualSiteRoleIds.contains(CONFIG.getProperty(multipleRoles[j].replace(' ', '_'))))
									{
										APP_LOGS.debug("Role did not Match. Quitting driver");										
										closeBrowser();
										assertTrue(false);
										return false;																													
										
									}
									
								}
								
								APP_LOGS.debug("Role Matched");
								
								//holds the control until the page is loaded completely
								waitForPageLoadForUser(actualSiteRoleIds);								
								
								return true;
								
								/*if(actualSiteRoleIds.contains(CONFIG.getProperty(xlsRole.replace(' ', '_'))))
								{
									
									APP_LOGS.debug("Role Matched");
									return true;
									
								}
								else 
								{
								
									APP_LOGS.debug("Role did not Match. Quitting driver");
									
									closeBrowser();
									assertTrue(false);
									return false;
																	
								}*/
								
							}
							else 
							{
								APP_LOGS.debug("Login failed at Login Window. Quitting Driver");
								closeBrowser();
								assertTrue(false);
								return false;
							}													
															
						}
						
					}
					
					APP_LOGS.debug("Role not found in Xls file. Quitting driver");
					
					closeBrowser();
					assertTrue(false);
					return false;
					
				}
				else 
				{
					APP_LOGS.debug("Role not provided. Quitting driver");
					closeBrowser();
					assertTrue(false);
					return false;
				}
				
				
			}
			catch(Throwable t)
			{
				t.printStackTrace();
				APP_LOGS.debug("Exception occurred in login function. Quitting driver");
				closeBrowser();
				assertTrue(false);
				return false;
				
			}
			
		
		}
		
		/**
		 * This function opens the site defined in the config.properties file and logins based on username and password provided and matches the role with the actual site role.
		 * If proper role name is not passed to the function, then closeBrowser() function is called which quits the driver and false is returned.
		 *  
		 * @param username	username of user with access to the site
		 * @param password	password of the user
		 * @param role		role from the following 'Admin' 'Version Lead' 'Test Manager' 'Tester' 'Stakeholder'. Note that Roles are case sensitive
		 * @return 			returns true if login is successfull and role matches on the site, otherwise returns false in any other condition
		 */
		public boolean login(String username, String password, String role)
		{
			
			String actualSiteRoleIds= null;
			//String siteRoleId = CONFIG.getProperty(role.replace(" ", "_"));
			
			APP_LOGS.debug("Trying login for role "+role); 
			
			if (!role.isEmpty()) 
			{				
				
				try
				{
					username = getCompleteUsername(username);					
					
					eventfiringdriver.get(CONFIG.getProperty("siteUrl"));
					
					
					if (UATLoginWindow(username, password)) 
					{						
						
						for (int i = 0; i < 25; i++) 
					    {
					       if (actualSiteRoleIds==null || actualSiteRoleIds.equals("[]") || actualSiteRoleIds.equals("[ ]")) 
					       {
					    	   Thread.sleep(1000);
					    	   try
					    	   {
					    		   actualSiteRoleIds = eventfiringdriver.executeScript("return security.userType").toString();
					    	   }
					    	   catch(Throwable t)
					    	   {
					         
					    	   }
					       	}
					       	else
					       		break;       
					        
					    }
						
						
						APP_LOGS.debug("Comparing selected role and site role");
						
						//in case user has multiple roles
						String[] multipleRoles = role.split("\\+");								
						
						for (int j = 0; j < multipleRoles.length; j++) 
						{									
							if(!actualSiteRoleIds.contains(CONFIG.getProperty(multipleRoles[j].replace(' ', '_'))))
							{
								APP_LOGS.debug("Role did not Match. Quitting driver");										
								closeBrowser();
								assertTrue(false);
								return false;																													
								
							}
							
						}
						
						APP_LOGS.debug("Role Matched");
						
						waitForPageLoadForUser(actualSiteRoleIds);							
						
						return true;					
						
											
						
						/*actualSiteRoleIds = (Object) eventfiringdriver.executeScript("return security.userType");
						
						APP_LOGS.debug("Comparing selected role and site role");
						
						
						if(actualSiteRoleIds.toString().contains(siteRoleId))
						{
							
							APP_LOGS.debug("Role Matched");
							return true;
							
						}
						else 
						{
						
							APP_LOGS.debug("Role did not Match. Quitting driver");
							
							closeBrowser();
							assertTrue(false);
							return false;
															
						}*/
						
					}
					else 
					{
						APP_LOGS.debug("Login failed at Login Window. Quitting Driver");
						closeBrowser();
						assertTrue(false);
						return false;
					}	
					
					
				}
				catch(Throwable t)
				{
					t.printStackTrace();
					APP_LOGS.debug("Exception occured in login function. Quitting driver");
					closeBrowser();
					assertTrue(false);
					return false;
					
				}
				
			}
			else 
			{
				APP_LOGS.debug("Role is not present. Quitting Driver");
				closeBrowser();
				assertTrue(false);
				return false;
			}
			
			
		
		}
		
		//to handle login windows based on environment
		private boolean UATLoginWindow(String username, String password) throws InterruptedException, AWTException
		{
			
			//Thread.sleep(1000);
			
			switch(CONFIG.getProperty("environment"))
			{
				case "Cloud":
				{
					 try
					 {
						 
						 /***The below code is commented as account option window is no longer available. 
						  ***UnComment if the first window is to select account option while login******/
						/* try
						 {	
							 eventfiringdriver.manage().timeouts().implicitlyWait(2, TimeUnit.SECONDS);
							 
							 if (eventfiringdriver.findElement(By.xpath(OR.getProperty("accountOption"))).isDisplayed()) 
							 {
								 eventfiringdriver.findElement(By.xpath(OR.getProperty("accountOption"))).click();
								 Thread.sleep(500);
							 }
							 
						 }
						 catch(Throwable t)
						 {
							 //System.out.println(t.getMessage());
						 }
						 finally
						 {
							 eventfiringdriver.manage().timeouts().implicitlyWait(Long.parseLong(CONFIG.getProperty("default_implicitWait")), TimeUnit.SECONDS);
							 							 
						 }*/
						 
						 
						 
						 
						 getElement("usernameInput").sendKeys(username);
							
				    	 getElement("passwordInput").sendKeys(password);		    	
				    	 
				    	 				    	 
				    	 //clicking once on submit button is not working, hence clicking again in case submit button button is still visible				    	
			    		 setImplicitWait(0);
			    		 
			    		 //eventfiringdriver.findElement(By.id(OR.getProperty("submitButton"))).click();
			    		 //eventfiringdriver.findElement(By.id(OR.getProperty("submitButton"))).click();
			    		 //Thread.sleep(3000);
			    		 
			    		 for (int i = 0; i < 3; i++) 
			    		 {
							try
							{
								//System.out.println("Click "+(i+1));
								eventfiringdriver.findElement(By.id(OR.getProperty("submitButton"))).click();
								
							}
							catch(Throwable t)
							{
								System.out.println("Error Caught\n"+t.getMessage());
								break;
							}
			    		 }
			    		 
			    		 //verifying whether login was unsuccessfull due to wrong credentials
			    		 /*try
			             {
			    			 //WebDriverWait wait1 = new WebDriverWait(eventfiringdriver, 10);	
			    			 System.out.println("Went Ahead");
			    			 wait1.until(ExpectedConditions.invisibilityOfElementLocated(By.id(OR.getProperty("submitButton"))));
			    			
			    			 APP_LOGS.debug("Login successfull at login window.");
			    			 
			    			 Thread.sleep(3000);
			    			 return true;
			             }
			             catch(Throwable t)
			             {
			            	 
			            	 APP_LOGS.debug("Submit button still visible. Credentials failed.");
			            	 //TestUtil.takeScreenShot("Login Failures", "Login Failed " +(loginFailureCounter++));
			            	 return false;
			             }	
			    		 finally
			    		 {
							resetImplicitWait();
			    		 }*/
			    		 
			    		 resetImplicitWait();
			    		 Thread.sleep(3000);
			    		 return true;
			    		 
				    		 
				    	 			    	 
						
				     }
				     catch(Throwable t)
				     {
				    	APP_LOGS.debug("Exception occured at login window.");
				    	t.printStackTrace();
						return false;
				     }
				}
				
				case "Local":
				{
					
					 Robot robot = new Robot();
					try
					{
						APP_LOGS.debug("On Login Window");
						Alert a = eventfiringdriver.switchTo().alert();
				        a.sendKeys(username);   
				        
				       
				        robot.keyPress(KeyEvent.VK_ENTER);                    
				        robot.keyPress(KeyEvent.VK_ENTER);   
				        
				        Thread.sleep(2000);
				        Alert b = eventfiringdriver.switchTo().alert();
				        b.sendKeys(password);
				    
				        robot.keyPress(KeyEvent.VK_ENTER);
				        robot.keyPress(KeyEvent.VK_ENTER);
				        
				        //this try block verifies whether login window pop up is still present in wrong credentials scenario
				        try
				        {
				        	Thread.sleep(500);
				        	eventfiringdriver.switchTo().alert();
				        	TestUtil.takeScreenShot("Login Failures", "Login Failed " +(loginFailureCounter++));
				        	robot.keyPress(KeyEvent.VK_ESCAPE);
				        	APP_LOGS.debug("Login Pop Up still visible. Credentials failed.");
				        	return false;
				        }
				        catch(Throwable t)
				        {
				        	APP_LOGS.debug("Login successfull at login window.");				        	
				        	Thread.sleep(3000);
				        	return true;
				        }
				        
				        
				        
					}
					catch(Throwable t)
					{
						try
				        {
				        	Thread.sleep(500);
				        	eventfiringdriver.switchTo().alert();
				        	TestUtil.takeScreenShot("Login Failures", "Login Failed " +(loginFailureCounter++));
				        	robot.keyPress(KeyEvent.VK_ESCAPE);
				        	APP_LOGS.debug("Login Pop Up still visible.");				        	
				        }
				        catch(Throwable e)
				        {
				        	
				        }						
						t.printStackTrace();
						APP_LOGS.debug("Exception occured at login window.");
						return false;
						
					}
				}
				
				default:
				{
					APP_LOGS.debug("Environment property is not correct");
					return false;
				}
			
			}
		
			
			
			
		}
		
		
		
		 		
		/**
		   * This function will get all the user credentials based on the role passed to it from User Xls file.
		   * Note that it returns ArrayList of a custom class type Credentials. So the reference variable, calling this function to get all desired role users, should be an ArrayList of type Credentials.
		   * After getting all the users in your ArrayList, use the statement 'variable_name.get(index).username' without quotes to get username for the provided index and for password just replace 'username' in the statement with 'password'.
		   * 
		   * @param role  role based on User Xls file.
		   * @param numOfUsers number of uers to fetch for the enterd role.
		   * @return   returns ArrayList of type Credentials if expected number of users are present for the provided role else returns null in any other condition.
		   * @see    Credentials 
		   */  
		  public ArrayList<Credentials> getUsers(String role, int numOfUsers)
		  {
			  int numOfRows;
			  String xlsRole;
			  ArrayList<Credentials> user = new ArrayList<Credentials>();
			  int index =0;
			  String sheetName = "Role";
		   
			  APP_LOGS.debug("Trying to get "+numOfUsers+" user(s) for role "+role);
			  
			  if (!role.isEmpty()) 
			  {
		    
				  numOfRows = userXls.getRowCount(sheetName);
		    
				  for (int i = 2; i <= numOfRows; i++) 
				  {
		     
					  xlsRole = userXls.getCellData(sheetName, "Role", i);
		     
					  if (role.equalsIgnoreCase(xlsRole)) 
					  {
		      
						  user.add(new Credentials());
		      
						  user.get(index).username= userXls.getCellData(sheetName, "Username", i);
		      
						  user.get(index).password= userXls.getCellData(sheetName, "Password", i);
		      
						  index++;
		      
						  if (index==numOfUsers) 
						  {
							  break;
		       
						  }
		      
					  }
		     
				  }	
		    
				  if (user.size()==numOfUsers) 
				  {
		     
					  return user;
		     
				  }
				  else 
				  {
		     
					  return null;
				  }
		    
			  }
			  else 
			  {
				  return null;
			  }
		   
		  }
		  
		  
		  
		  
		  
		  
		  /**
		   * This function will get the first available user credentials based on the role passed to it from User Xls file from User sheet. the user will already have access
		   * Note that it returns object of a custom class Credentials. So the reference variable, calling this function to get all desired role users, should be of class Credentials.
		   * After getting the user in your variable object, use the statement 'variable_name.username' without quotes to get username and for password just replace 'username' in the statement with 'password'.
		   * 
		   * @param role  role based on User Xls file.
		   * @return   returns object of class Credentials if the user is present for the provided role in Users sheet of User Xls file else returns null in any other condition.
		   * @see    Credentials 
		   */  
		  public Credentials getUserWithAccess(String role)
		  {
			  int numOfRows;
			  Credentials user = null;
			  String xlsRole;				  
			  String sheetName = "Users";
		   
			  APP_LOGS.debug("Trying to get user for role "+role+" from "+sheetName+" sheet.");
			  			 
			  try 
			  {
				  
				  numOfRows = userXls.getRowCount(sheetName);
				    
				  for (int i = 2; i <= numOfRows; i++) 
				  {
			     
					  xlsRole = userXls.getCellData(sheetName, "Role", i);
			     
					  if (role.equalsIgnoreCase(xlsRole)) 
					  {
						  user = new Credentials(userXls.getCellData(sheetName, "Username", i), userXls.getCellData(sheetName, "Password", i));
			      
						  break;   						  
			      
					  }
			     
				  }	
				
			  } 
			  catch (Throwable t) 
			  {
				APP_LOGS.debug("Exception in 'getUserToLogin(String role)' function");
				t.getMessage();
			  }
			  
				  
			  return user;				  
		    			  
		   
		  }
		  
		  
		  
		  
		  
		
		
		//reading xpath of element from object repository file
		  public WebElement getObject(String xpathKey)
		  {			  
			  
			  try
			  {
				
				WebElement x = eventfiringdriver.findElement(By.xpath(OR.getProperty(xpathKey)));
				 				
				return x;				
				  
			  }
			  catch(Throwable t)
			  {
				  
				  stacktrace = Thread.currentThread().getStackTrace();
				  stackElement = stacktrace[2];
				  className = stackElement.getClassName();
				  String[] extractTestCaseName = className.split("\\.");
				  className = extractTestCaseName[extractTestCaseName.length-1];
				  TestUtil.takeScreenShot(className, xpathKey);				  
				  
				  //ErrorUtil.addVerificationFailure(t); 
				  
				  APP_LOGS.debug("Cannot find object with key -- " +xpathKey);
				  return null;
			  }
		   
		  }
		  
		  
		       
		  //reading id of element from object repository file
		  public WebElement getElement(String idKey) throws IOException
		  {
		     try
		     {
		          WebElement x= eventfiringdriver.findElement(By.id(OR.getProperty(idKey)));
		           
		          return x;
					
		           
		     }
		     catch(Throwable t)
		     {
		    	 stacktrace = Thread.currentThread().getStackTrace();
				  stackElement = stacktrace[2];
				  className = stackElement.getClassName();
				  String[] extractTestCaseName = className.split("\\.");
				  className = extractTestCaseName[extractTestCaseName.length-1];
				  
				 TestUtil.takeScreenShot(className, idKey);
		    	 //report error     
		    	 //ErrorUtil.addVerificationFailure(t);   
		    	 APP_LOGS.debug("Cannot find element with key -- " +idKey);
		    	 return null;
		     }
		  }
		  
		  //Added by Swati Gangrade on date 01st Nov 2014 for the selction of month and year from date picker using its class attribute
		  
		  //reading id of element from object repository file
		  
		  public WebElement getElementByClassAttr (String classKey) throws IOException
		  {
		     try
		     {
		    	 
		    	 WebElement x= eventfiringdriver.findElement(By.className(OR.getProperty(classKey)));
		           
		    	 return x;		        
		           
		     }
		     catch(Throwable t)
		     {
		    	  stacktrace = Thread.currentThread().getStackTrace();
				  stackElement = stacktrace[2];
				  className = stackElement.getClassName();
				  String[] extractTestCaseName = className.split("\\.");
				  className = extractTestCaseName[extractTestCaseName.length-1];
				  
				 TestUtil.takeScreenShot(className, classKey);
				 //report error     
				 //ErrorUtil.addVerificationFailure(t);   
				 APP_LOGS.debug("Cannot find element with key -- " +classKey);
				 return null;
		     }
		  }
	
		 //for collective xpaths
		  public WebElement getObject(String xpathKey1,String xpathKey2, int index )
		  {
			  
			  String collectiveXpath = OR.getProperty(xpathKey1)+index+OR.getProperty(xpathKey2);
			   
			  try
			  {
				  WebElement x= eventfiringdriver.findElement(By.xpath(collectiveXpath));
		           		         
				  return x;		         			  
			    
			  }
			  catch(Throwable t)
			  {
				  stacktrace = Thread.currentThread().getStackTrace();
				  stackElement = stacktrace[2];
				  className = stackElement.getClassName();
				  String[] extractTestCaseName = className.split("\\.");
				  className = extractTestCaseName[extractTestCaseName.length-1];
				  
				  TestUtil.takeScreenShot(className, xpathKey1+"+"+index+"+"+xpathKey2);
				  // report error
				  
				  //ErrorUtil.addVerificationFailure(t);  
			    
				  APP_LOGS.debug("Cannot find object with key -- " +(xpathKey1+"+"+index+"+"+xpathKey2));
			    
				  return null;
			  }
			   
		  }
		  
		  
		  
		  
		  /****
		     * This function uses javascript to click the element passed to it as parameter and returns the control 
		     * instantly after clicking for the next statement to be executed
		     * 
		     * @param element to click on
		     * @throws NullPointerException
		     */
		  public void click(WebElement element)
		  {
			  if (element != null)
				  eventfiringdriver.executeScript("var el = arguments[0]; el.click();", element);
			  else
				  throw new NullPointerException();			  
		  }
		  
		  
		  
		  
		  
		  /**
		   * Gets text from the auto hide pop ups (alerts) used after save, update and delete actions. 
		   * Has dynamic waits for visbility and invisiblity of the pop up.
		   * 
		   * @return text from the auto hide pop up and null in any other case
		   */
		  protected String getTextFromAutoHidePopUp()
		  {
			  
			  String text = null;			  
			  
			  try
			  {		
				  //WebDriverWait wait1 = new WebDriverWait(eventfiringdriver, 10);
				  
				  setImplicitWait(0);				  				  
				  text = wait1.until(ExpectedConditions.visibilityOf(eventfiringdriver.findElement(By.id("autoHideAlert")))).getText();
				  
				  wait1.until(ExpectedConditions.invisibilityOfElementLocated(By.id("autoHideAlert")));
				  Thread.sleep(500);
			  
			  }
			  catch(Throwable t)
			  {
				  //t.printStackTrace();
			  }
			  finally
			  {
				  resetImplicitWait();
			  }
		    
			  return text;
			  
		  }
		  
		  
		  //gets text from the alert(element) using dynamic wait for alert to show up
		 /* protected String getTextFromAlert(WebElement element)
		  {
			  
			  String text = null;			  
			  
			  try
			  {		
				  WebDriverWait wait1 = new WebDriverWait(eventfiringdriver, 10);
				  
				  setImplicitWait(0);
				  				  
				  text = wait1.until(ExpectedConditions.visibilityOf(element)).getText();
				  
			  
			  }
			  catch(Throwable t)
			  {
				  t.printStackTrace();
			  }
			  finally
			  {
				 resetImplicitWait();
			  }
		    
			  return text;
			  
		  }*/
		  
		  
		 /* public void printTime(String text)
		  {
			  calendar = calendar.getInstance();
			  System.out.println(text+ " " +Time.format(calendar.getTime()));
		  }*/
		  
		  
	
		  //utility for login fucntion to get the complete username based on domain
		  private String getCompleteUsername(String username)
		  {
			  if (CONFIG.getProperty("domain").contains(".net")) 
					username += "@" + CONFIG.getProperty("domain");					
				else
					username = CONFIG.getProperty("domain")+"\\"+username;
			  
			  return username;
		  }
		  
		  
		  
		  //a dynamic wait for login function to wait till the page has loaded completely for further action, works based on user
		  private void waitForPageLoadForUser(String siteRoleIDs) throws InterruptedException
		  {
			  By locator;
			  boolean landsOnDashboard = true;
			  
			  if (siteRoleIDs.contains(CONFIG.getProperty("Stakeholder")) && !siteRoleIDs.contains(CONFIG.getProperty("Tester")))
				  landsOnDashboard = false;
			 
			  
			  if (landsOnDashboard) 
				  locator = By.id(OR.getProperty("dashboardElementID"));
			  else
				  locator = By.id(OR.getProperty("stakeholderDashboardElementID"));
			  
			  wait.until(ExpectedConditions.presenceOfElementLocated(locator));
			  wait.until(ExpectedConditions.visibilityOfElementLocated(locator));
			  
			  Thread.sleep(500);
		  }
		  
		  
		  
		  //a dynamic wait that will wait for element's visibility located by the config key and will return element on visibility
		  /**
		   * 
		   * @param key of the element from the object repository to wait for visibility
		   * @param timeOutInSeconds if the element doesn't show up for specified time
		   * @return element after it becomes visible else returns null after the time out
		   * @throws InterruptedException
		   */
		  public WebElement waitForElementVisibility(String key, long timeOutInSeconds) throws InterruptedException
		  {
			  By locator;
			  boolean isXpath = true;
			  WebElement element = null;
			  
			  
			  try
			  {
				  
				  WebDriverWait w = new WebDriverWait(eventfiringdriver, timeOutInSeconds);
				  
				  setImplicitWait(0);
				  
				  if (!OR.getProperty(key).contains("/"))
					  isXpath = false;				 
				  
				  if (isXpath) 
					  locator = By.xpath(OR.getProperty(key));
				  else
					  locator = By.id(OR.getProperty(key));
				  
				  w.until(ExpectedConditions.presenceOfElementLocated(locator));
				  element = w.until(ExpectedConditions.visibilityOfElementLocated(locator));
				  
			  }
			  catch(Throwable t)
			  {
				  t.printStackTrace();
			  }
			  finally
			  {
				  resetImplicitWait();
			  }			 
			  
			  //Thread.sleep(500);
			  
			  return element;
			  
		  }
		  
		  
		  /**
		   * Used to set implicit wait when required. After its usage resetImplicitWait function should be called compulsorily for default timeout
		   * 
		   * @param time to set implicit wait for
		   */
		  public void setImplicitWait(long time)
		  {
			  eventfiringdriver.manage().timeouts().implicitlyWait(time, TimeUnit.SECONDS);
		  }
		  
		  
		  /**
		   * Used to reset implicit wait as per the default time in the config file
		   */
		  public void resetImplicitWait()
		  {
			  eventfiringdriver.manage().timeouts().implicitlyWait(Long.parseLong(CONFIG.getProperty("default_implicitWait")), TimeUnit.SECONDS);
		  }
	
	

}
