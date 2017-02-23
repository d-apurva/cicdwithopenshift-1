/**
 * 
 */
package com.uat.pages;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;

/**
 * @author Admin
 *
 *
 *  This method will store all the locators and methods of login page
 *
 */
public class LoginPage 
{
	WebDriver driver;

		By username= By.id("Email");
		By password=By.id("Password");
		By signinButton = By.id("Login");
		By testManagement_Id= By.id("testMgnt");
		By use_another_account_button = By.className("use_another_account");
		
		public LoginPage(WebDriver driver)
		{
			this.driver=driver;
		}
		
		public void typeUsername(String Username)
		{
			driver.findElement(username).sendKeys(Username);
		}
		
		public void typePassword(String Password)
		{
			driver.findElement(password).sendKeys(Password);
		}
		
		public void clickSignInButton()
		{
			driver.findElement(signinButton).click();
		}
		public void clicktestManagement()
		{
			driver.findElement(testManagement_Id).click();
		}
		
		public void CloudLogin(String User, String Pass)
		{
			/**try
			{
				driver.findElement(use_another_account_button).click();
			}
			catch(Throwable t)
			{
				System.out.println("Test");
			}**/
			
			driver.findElement(username).sendKeys(User);
			driver.findElement(password).sendKeys(Pass);
			
			
			/**for (int i = 0; i < 3; i++) 
			{
				try
				{
					//System.out.println("Click "+(i+1));
					clickSignInButton();
					
				}
				catch(Throwable t)
				{
					System.out.println("Error Caught\n"+t.getMessage());
					break;
				}
			}**/
			clickSignInButton();
		}
		
		
}
