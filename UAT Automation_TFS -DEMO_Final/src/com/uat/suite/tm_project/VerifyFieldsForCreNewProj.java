/*Author Name:-SwatiGangarde
 * Created Date:-31st of Oct 2014
 * Last Modified on Date:-05th Nov 2014
 * Module Description:- Creation of project with the positive data and handeling the validation message if any i=of the mandatory field is blank
 */


package com.uat.suite.tm_project;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.testng.SkipException;
import org.testng.annotations.AfterMethod;
import org.testng.annotations.AfterTest;
import org.testng.annotations.BeforeTest;
import org.testng.annotations.DataProvider;
import org.testng.annotations.Listeners;
import org.testng.annotations.Test;

import recorder.Utility;

import com.uat.base.Credentials;
import com.uat.util.TestUtil;

@Listeners(com.uat.listener.TestsListenerAdapter.class)
public class VerifyFieldsForCreNewProj extends TestSuiteBase
{
	String runmodes[]=null;
	boolean fail=false;
	boolean skip=false;
	boolean isTestPass=true;
	boolean isLoginSuccess=true;
	int count=-1;
	String comments;
	ArrayList<Credentials> versionLead;
	ArrayList<Credentials> stakeholder;
	Utility utilRecorder = new Utility();
	
	// Runmode of test case in a suite
	@BeforeTest
	public void checkTestSkip() throws Exception
	{
		APP_LOGS.debug("Beginning test case '"+this.getClass().getSimpleName()+"'.");
		
		if(!TestUtil.isTestCaseRunnable(TM_projectSuiteXls,this.getClass().getSimpleName()))
		{
			APP_LOGS.debug("Skipping Test Case"+this.getClass().getSimpleName()+" as runmode set to NO");//logs
			throw new SkipException("Skipping Test Case"+this.getClass().getSimpleName()+" as runmode set to NO");//reports
		}
		runmodes=TestUtil.getDataSetRunmodes(TM_projectSuiteXls, this.getClass().getSimpleName());
		utilRecorder.startRecording(System.getProperty("user.dir")+"\\Videos\\"+extractPackageName(this.getClass().getPackage().toString())+"\\"+this.getClass().getSimpleName());

	}
	
	@Test(dataProvider="getTestData")
	public void verifyFieldsForCreNewProj(
			String role, String GroupName,String PortfolioName,String ProjectName, String Version, String Description, 
			String StartMonth, String StartYear, String StartDate,String EndMonth, String EndYear, String EndDate, 
			String VersionLead, String Stakeholder, String ProjectURL,String AliasProjectURL, String ApplicationURL, 
			String AliasApplicationURL, String ExpectedBlankValidatiionMessage ,String ExpectedMessageText ) throws Exception
	{
		count++;
		comments="";
		
		if(!runmodes[count].equalsIgnoreCase("Y")){
			skip=true;
			APP_LOGS.debug("Runmode for test set data no. "+(count+1)+" set to no.........So its skipping Test Set Data No."+(count+1));;
			
			throw new SkipException("Runmode for test set data no. "+(count+1)+" set to no");
		}
		
		int versionlead_count = Integer.parseInt(VersionLead);
		int stakeholder_count = Integer.parseInt(Stakeholder);
		
		
		
		versionLead = getUsers("Version Lead", versionlead_count);
		stakeholder = getUsers("Stakeholder", stakeholder_count);
		
		openBrowser();	
		
		APP_LOGS.debug("Browser is getting opened... ");
		
		APP_LOGS.debug("Calling Login with role "+ role);
	
		
		
		isLoginSuccess = login(role);
		
		if(isLoginSuccess)
		{
			APP_LOGS.debug("Login Successfull");
			
			APP_LOGS.debug("Executing Test VerifyFieldsForCreNewProj");	
			
			try
			{
				getElement("UAT_testManagement_Id").click();
				
				Thread.sleep(500);
				
				WebDriverWait wait = new WebDriverWait(driver, 1000);
				
				wait.until(ExpectedConditions.elementToBeClickable(By.xpath("//div[@id='pageTab']/h2[2]")));
				//wait.until(ExpectedConditions.elementToBeClickable(By.xpath(OR.getProperty("Projects_createNewProjectLink"))));
				
												
				getObject("Projects_createNewProjectLink").click();
				
				//Without entering or selecting group name clicking in Save button validation message appears for that using the function viz.'validationMessageExceptVersionLead'
				APP_LOGS.debug("Verifying Group is not selected ...");
				if (validationMessageExceptVersionLead(getObject("ProjectCreateNew_projectSaveBtn"),ExpectedBlankValidatiionMessage,getElement("ProjectCreateNew_validationMessage_Id"))) 
				{
					comments ="Empty Group Validation Successful(Pass). ";
				}
				else 
				{
					fail=true;
					comments ="Empty Group Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Group Validation");
					
				}
				
				
				//After validation message pop up selecting or creating group using this function viz. 'SelectOrCreationGroupAndPortfolioAndProject'
				SelectOrCreationGroupAndPortfolioAndProject(getElement("ProjectCreateNew_groupDropDown_Id"),getObject("ProjectCreateNew_groupAddButton"), GroupName );
			
				//Without entering or selecting portfolio name clicking in Save button validation message appears for that using the function viz.'validationMessageExceptVersionLead'
				APP_LOGS.debug("Verifying Portfolio is not selected ...");
				
				if (validationMessageExceptVersionLead(getObject("ProjectCreateNew_projectSaveBtn"),ExpectedBlankValidatiionMessage, getElement("ProjectCreateNew_validationMessage_Id")))
				{
					comments+="Empty Potfolio Validation Successful(Pass). ";
				}
				else 
				{
					fail=true;
					comments+="Empty Potfolio Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Portfolio Validation");
					
				}
				
				
				//After validation message pop up selecting or creating portfolio using this function viz. 'SelectOrCreationGroupAndPortfolioAndProject'
				SelectOrCreationGroupAndPortfolioAndProject(getElement("ProjectCreateNew_PortfolioDropDown_Id"),getObject("ProjectCreateNew_portfolioAddButton"), PortfolioName);
				
				//Without entering or selecting project name clicking in Save button validation message appears for that using the function viz.'validationMessageExceptVersionLead'
				APP_LOGS.debug("Verifying Project is not selected ...");
				
				
				if (validationMessageExceptVersionLead(getObject("ProjectCreateNew_projectSaveBtn"),ExpectedBlankValidatiionMessage,getElement("ProjectCreateNew_validationMessage_Id")))
				{
					comments+="Empty Project Validation Successful(Pass). ";
				}
				else 
				{
					fail=true;
					comments+="Empty Project Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Project Validation");
					
				}
				
				//After validation message pop up selecting or creating project using this function viz. 'SelectOrCreationGroupAndPortfolioAndProject'
				SelectOrCreationGroupAndPortfolioAndProject(getElement("ProjectCreateNew_projectDropDown_Id"),getObject("ProjectCreateNew_projectAddButton"), ProjectName );
				
				//Enter version data 		
				APP_LOGS.debug("Verifying version field is blank ...");				
				
				if (validationMessageExceptVersionLead(getObject("ProjectCreateNew_projectSaveBtn"),ExpectedBlankValidatiionMessage,getElement("ProjectCreateNew_validationMessage_Id")))
				{
					comments+="Empty Version Validation Successful(Pass). ";
				}
				else 
				{
					fail=true;
					comments+="Empty Version Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Version Validation");
					
				}
				
				APP_LOGS.debug("Entering Version Name...");
				getObject("ProjectCreateNew_versionTextField").sendKeys(Version);		
				
				//To enter the description 
				APP_LOGS.debug("Description is getting entered...");
				getElement("ProjectCreateNew_descriptionTextField_Id").sendKeys(Description);
				Thread.sleep(1000);
				
				//Version Lead selection
				APP_LOGS.debug("Verifying version lead field is blank ...");
				
				if (validationForVersionLeadAndProjectSuccess(getObject("ProjectCreateNew_projectSaveBtn"),"Version Lead cannot be empty!"))
				{
					comments+="Empty Version Lead Validation Successful(Pass). ";
				}
				else 
				{
					fail=true;
					comments+="Empty Version Lead Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Version Lead Validation");
					
				}				
				toSelectVersionLeadfromPeoplePicker(getObject("ProjectCreateNew_versionLeadPeoplePickerImg"),versionLead.get(0).username);
						
				
				//End Date selection 	
				APP_LOGS.debug("Verifying End Date mandatory field is blank ...");
				
				if (validationMessageExceptVersionLead(getObject("ProjectCreateNew_projectSaveBtn"),ExpectedBlankValidatiionMessage, getElement("ProjectCreateNew_validationMessage_Id")))
				{
					comments+="Empty End Date Validation Successful(Pass). ";
				}
				else 
				{
					fail=true;
					comments+="Empty End Date Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "End Date Validation");
					
				}
								
					//To select end date from End Date Picker 
				toSelectStartDateandEndDate(getObject("ProjectCreateNew_startDateImage"),StartMonth,StartYear, StartDate);
				toSelectStartDateandEndDate(getObject("ProjectCreateNew_endDateImage"),EndMonth,EndYear, EndDate);
				
				
				
				
				//Stakeholder selection
				toSelectStakeholderfromPeoplePicker(getObject("ProjectCreateNew_StakeholderPeoplePickerImg"),stakeholder.get(0).username);
				
				
				
				//Project URL
				
				//Clicking on Project URL image icon 
				Thread.sleep(1000);
				getObject("ProjectCreateNew_projectURLImageIcon").click();			
				getElement("ProjectCreateNew_projectURLTextField_Id").sendKeys(ProjectURL);		
				getElement("ProjectCreateNew_projectAliasURLTextField_Id").sendKeys(AliasProjectURL);		
				getObject("ProjectCreateNew_projectAliasURLOKBtn").click();
						
				
			
				//Application URL
				
				//Clicking on Application URL image icon 
				Thread.sleep(1000);
				getObject("ProjectCreateNew_applicationURLImageIcon").click();			
				getElement("ProjectCreateNew_applicationURLTextField_Id").sendKeys(ApplicationURL);			
				getElement("ProjectCreateNew_applicationAliasURLTextField_Id").sendKeys(AliasApplicationURL);			
				getObject("ProjectCreateNew_applicationAliasURLOKBtn").click();
					
				comments+="Project Success Save Validation Successful(Pass). ";
				
				
			
				//Clicked on Save button after entering test data in all mandatory and non mandatory text fields
				if (validationForVersionLeadAndProjectSuccess(getObject("ProjectCreateNew_projectSaveBtn"), ExpectedMessageText))
				{					
					closeBrowser();
					
					openBrowser();
					
					Thread.sleep(4000);
					
					if (login(versionLead.get(0).username, versionLead.get(0).password, "Version Lead")) 
					{
						
						getElement("UAT_testManagement_Id").click();
						
						Thread.sleep(3000);
								
						if (assertTrue(searchProjectAndEdit(ProjectName, Version))) 
						{
							
							comments+="Project Creation Successful and project found in View All table (Pass). ";
							
							if (!verifyProjectUpdatedFields(GroupName, PortfolioName, ProjectName, Version, Description, StartMonth,
									StartYear, StartDate, EndMonth, EndYear, EndDate,versionLead.get(0).username, 1, 
									ProjectURL, AliasProjectURL, ApplicationURL, AliasApplicationURL)) 
							{
								
								APP_LOGS.debug("Created Project Verification Failed");
								comments+="Created Project Verification Failed(Fail). ";
								assertTrue(false);
								TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Verfication failure");
								fail =true;
								
								
							}
							else 
							{
								comments+="Created Project Verification Success(Pass). ";
							}
							
							
						}
						else
						{
							fail= true;
							TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Project not found in View All Table");
							comments+="Project Creation Successful but project not found in View All table (Fail). ";
						}
						
						wait = new WebDriverWait(driver, 1000);
						
						wait.until(ExpectedConditions.elementToBeClickable(By.xpath("//div[@id='pageTab']/h2[2]")));
						
						getObject("Projects_createNewProjectLink").click();
						
						getElement("ProjectCreateNew_descriptionTextField_Id").sendKeys(Description);
						
						//Stakeholder selection
						toSelectStakeholderfromPeoplePicker(getObject("ProjectCreateNew_StakeholderPeoplePickerImg"),stakeholder.get(0).username);
						
						//Project URL
					
						//Clicking on Project URL image icon 
						getObject("ProjectCreateNew_projectURLImageIcon").click();		
						wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("actualURL")));
						getElement("ProjectCreateNew_projectURLTextField_Id").sendKeys(ProjectURL);		
						getElement("ProjectCreateNew_projectAliasURLTextField_Id").sendKeys(AliasProjectURL);		
						getObject("ProjectCreateNew_projectAliasURLOKBtn").click();
						
						/*//Application URL
						
						//Clicking on Application URL image icon 
						getObject("ProjectCreateNew_applicationURLImageIcon").click();		
						wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("actualURL2")));
						getElement("ProjectCreateNew_applicationURLTextField_Id").sendKeys(ApplicationURL);			
						getElement("ProjectCreateNew_applicationAliasURLTextField_Id").sendKeys(AliasApplicationURL);			
						//getObject("ProjectCreateNew_applicationAliasURLOKBtn").click();
						getObject("ProjectCreateNew_projectAliasURLOKBtn").click();*/
						
						Thread.sleep(1000);
						//Clicked on Save button after entering test data in all mandatory and non mandatory text fields
						if (validationMessageExceptVersionLead(getObject("ProjectCreateNew_projectSaveBtn"), ExpectedBlankValidatiionMessage,getElement("ProjectCreateNew_validationMessage_Id")))
						{
							comments+="Only non mandatory fields' filled Validation Successful(Pass). ";
						}
						else 
						{
							fail=true;
							comments+="Only non mandatory fields' filled Validation UnSuccessful(Fail). ";
							TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Non mandatory field Validation");
							
						}						
						
						
					}
					else 
					{
						fail= true;
						comments+="Login Failed for assigned Version Lead(Fail). ";
						
					}				
					
					
				}
				else 
				{
					fail=true;
					comments+="Project Success Save Validation UnSuccessful(Fail). ";
					TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Project Success Save Validation");
					
				}		
			
				
				
			
							
			}
			catch(Throwable t)
			{
				fail= true;
				t.printStackTrace();
				assertTrue(false);
				comments+="Exception Occured. ";
				TestUtil.takeScreenShot(this.getClass().getSimpleName(), "Exception");
				APP_LOGS.debug("Browser is getting closed...");
				
			}		
			
			closeBrowser();
				
		
		}
					
		
	}
	
	@AfterMethod
	public void reportDataSetResult(){
		if(skip)
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "SKIP");
		else if(!isLoginSuccess){
			isTestPass=false;
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "Login UnSuccessfull");
		}
		else if(fail){
			isTestPass=false;
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "FAIL");
			TestUtil.printComments(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, comments);
		}
		else{
			TestUtil.reportDataSetResult(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, "PASS");
			TestUtil.printComments(TM_projectSuiteXls, this.getClass().getSimpleName(), count+2, comments);
		}
			
		
		skip=false;
		fail=false;		

	}
	
	
	@AfterTest
	public void reportTestResult() throws Exception{
		if(isTestPass)
			TestUtil.reportDataSetResult(TM_projectSuiteXls, "Test Cases", TestUtil.getRowNum(TM_projectSuiteXls,this.getClass().getSimpleName()), "PASS");
		else
			TestUtil.reportDataSetResult(TM_projectSuiteXls, "Test Cases", TestUtil.getRowNum(TM_projectSuiteXls,this.getClass().getSimpleName()), "FAIL");
		
		utilRecorder.stopRecording();
		
	}
	
	
	
	@DataProvider
	public Object[][] getTestData(){
		return TestUtil.getData(TM_projectSuiteXls, this.getClass().getSimpleName()) ;
	}
	
	
	
	private boolean searchProjectAndEdit(String project, String version) throws IOException, InterruptedException
	{
		int totalPages;
		int projectsOnPage;
		String gridProject;
		String gridVersion;
		//int projectFoundFlag=0;
		APP_LOGS.debug("Searching Project to Edit");
		
		try
		{
		
			if (getElement("ProjectViewAll_Pagination_Id").findElements(By.xpath("div/span")).size()==3) 
			{
				APP_LOGS.debug("Only 1 page available on View All page.");
				
				totalPages=1;
				
			}
			else 
			{
				APP_LOGS.debug("More than 1 page avaialble on View All page. Calculating total pages.");
				
				totalPages = getElement("ProjectViewAll_Pagination_Id").findElements(By.xpath("div/a")).size();
			}		
			
			for (int i = 0; i < totalPages; i++) 
			{
				projectsOnPage = getElement("ProjectViewAll_Table_Id").findElements(By.xpath("tr")).size();
				
				for (int j = 1; j <= projectsOnPage; j++) 
				{
					gridProject=getObject("ProjectViewAll_projectNameColumn1", "ProjectViewAll_projectNameColumn2", j).getAttribute("title");
					gridVersion= getObject("ProjectViewAll_projectVersionColumn1", "ProjectViewAll_projectVersionColumn2", j).getText();
					
					if (gridProject.equals(project) && gridVersion.equals(version)) 
					{
						getObject("ProjectViewAll_editProjectIcon1", "ProjectViewAll_editProjectIcon2", j).click();
						Thread.sleep(1000);
						APP_LOGS.debug("Project Found in View All page.");
						return true;
					}
					
					
				}			
				
				if (totalPages>1 && i!=(totalPages-1)) 
				{
					getObject("ProjectViewAll_NextLink").click();
				}
				
			}
			
			//assertTrue(false);
			APP_LOGS.debug("Project Not found in View All page");
			
			return false;
		}
		catch(Throwable t)
		{
			t.printStackTrace();
			//assertTrue(false);
			APP_LOGS.debug("Exception in searchProjectAndEdit(). ");
			return false;
		}
		
	}
	
	
	
	
	
	private boolean verifyProjectUpdatedFields(String group, String portfolio, String project, String version,String description, String startMonth, 
			String startYear, String startDate, String endMonth, String endYear, String endDate, String versionLead, 
			int stakeholdersCount, String projectUrl, String projectAlias, String appUrl, String appAlias) throws IOException, InterruptedException
	{	
		Select groupDropDown;
		String displayedGroup;
		Select portfolioDropDown;
		String displayedPortfolio;
		//Select projectDropDown;
		String displayedProject;
		String displayedVersion;
		String displayedDescription;
		String selectedStartDate;
		String selectedEndDate;
		String displayedVersionLead;
		int displayedStakeholdersCount;
		String displayedProjectUrl;		
		String displayedProjectAlias;
		String expectedStartDate = getMonthNumber(startMonth)+"/"+dateHandler(startDate)+"/"+startYear;
		String expectedEndDate= getMonthNumber(endMonth)+"/"+dateHandler(endDate)+"/"+endYear;
		versionLead= versionLead.replace(".", " ");
		
		try
		{	
			
			groupDropDown = new Select(getElement("ProjectCreateNew_groupDropDown_Id"));
			displayedGroup= groupDropDown.getFirstSelectedOption().getText();
			
			portfolioDropDown= new Select(getElement("ProjectCreateNew_PortfolioDropDown_Id"));
			displayedPortfolio= portfolioDropDown.getFirstSelectedOption().getText();
			
			//projectDropDown= new Select(getElement("ProjectCreateNew_projectDropDown_Id"));
			displayedProject= getElement("ProjectCreateNew_projectDropDown_Id").getAttribute("value");
			
			displayedVersion=getObject("ProjectCreateNew_versionTextField").getAttribute("value");
			
			displayedDescription=getElement("ProjectCreateNew_descriptionTextField_Id").getText();
			
			selectedStartDate= getElement("ProjectCreateNew_startDateField_Id").getAttribute("value");
			
			selectedEndDate=getElement("ProjectCreateNew_endDateField_Id").getAttribute("value");
			
			displayedVersionLead=getObject("ProjectCreateNew_versionLeadDisplayField").getText();
			
			displayedStakeholdersCount= eventfiringdriver.findElement(By.xpath("//div[@id='ctl00_ctl00_midPannel_ContentPlaceHolder1_Child_ContentPlaceHolder1_PeopleEditorUser4Port_upLevelDiv']")).findElements(By.xpath("span")).size();
			
			getObject("ProjectCreateNew_projectURLEditButton").click();
			
			displayedProjectUrl=getElement("ProjectCreateNew_projectURLPopUpTextField_Id").getAttribute("value");
			
			displayedProjectAlias=getElement("ProjectCreateNew_projectAliasPopUpTextField_Id").getAttribute("value");
			
			getObject("ProjectCreateNew_projectURLPopUpOkButton").click();
			
			Thread.sleep(500);
			
	/*		getObject("ProjectCreateNew_appURLEditButton").click();
			
			displayedApplicationUrl=getElement("ProjectCreateNew_appURLPopUpTextField_Id").getAttribute("value");
			
			displayedApplicationAlias=getElement("ProjectCreateNew_appAliasPopUpTextField_Id").getAttribute("value");
			
			
			//getObject("ProjectCreateNew_appURLPopUpOkButton").click();
			eventfiringdriver.findElement(By.className("ui-dialog-titlebar-close ui-corner-all")).click();
			
			Thread.sleep(500);*/
			
			if (assertTrue(displayedGroup.equals(group) && displayedPortfolio.equals(portfolio) && displayedProject.equals(project) &&
					displayedVersion.equals(version) && displayedDescription.equals(description) && selectedStartDate.equals(expectedStartDate) && 
					selectedEndDate.equals(expectedEndDate) && displayedVersionLead.equalsIgnoreCase(versionLead) && displayedStakeholdersCount==stakeholdersCount && 
					displayedProjectUrl.equals(projectUrl) && displayedProjectAlias.equals(projectAlias))) 
			{
				return true;
				
			}
			
			return false;
								
		}
		catch(Throwable t)
		{
			t.printStackTrace();
			return false;
		}	
	
	
	}
	
	
	
	private String getMonthNumber(String month)
	{
		
		
		switch (month) {
		case "Jan":
			return "01";
		case "Feb":
			return "02";
		case "Mar":
			return "03";
		case "Apr":
			return "04";
		case "May":
			return "05";
		case "Jun":
			return "06";
		case "Jul":
			return "07";
		case "Aug":
			return "08";
		case "Sep":
			return "09";
		case "Oct":
			return "10";
		case "Nov":
			return "11";
		case "Dec":
			return "12";

		default:
			return null;
		}
	}
	
	private String dateHandler(String date)
	{
		int idate = Integer.parseInt(date);
		
		if (idate<10) 
		{
			date="0"+date;
		}
		
		return date;
	}
		
	//Verifying if the Portfolio name is present in Portfolio/Process Name drop down , if it is present then select it  if not present then create it
	public void SelectOrCreationGroupAndPortfolioAndProject(WebElement DropDownList, WebElement AddButton, String InputtedTestData) throws IOException
	{
		
		List<WebElement> elements = DropDownList.findElements(By.tagName("option"));
		int flag = 0;
		
		for(int i =0 ;i<elements.size();i++)
		{
			if(elements.get(i).getText().equals(InputtedTestData))
			{
				flag++;
				elements.get(i).click();
				break;
			}
		}
		if(flag==0)
		{
			//Click on Plus icon to add Group or Portfolio
			AddButton.click();
				
			getElement("ProjectCreateNew_groupPortfolioProjectTextField_Id").clear();
			getElement("ProjectCreateNew_groupPortfolioProjectTextField_Id").sendKeys(InputtedTestData);
			
			//To validate the blank input test data if the test data is not blank then go here in IF
			if (getElement("ProjectCreateNew_projectAddPopUpLabel_Id").getText().contains("Project")) 
			{
				getObject("ProjectCreateNew_projectAddBtn").click();
			}
			else 
			{
				getObject("ProjectCreateNew_groupPortfolioSaveBtn").click(); 
			}

		}
	}
	
	//Function to select the Start date and end date from date picker 
	public void toSelectStartDateandEndDate(WebElement startEndDateImage, String StartEndMonth, String StartEndYear,String StartEndDate) throws IOException
	{
		try
		{
			//Select start date 		
			WebElement startDateImage = startEndDateImage;		
			startDateImage.click();		
			APP_LOGS.debug("Clicked on Date Calendar icon...");
			
			Select year = new Select(getElementByClassAttr("ProjectCreateNew_yearDropDown_Class"));
			year.selectByValue(StartEndYear);
			APP_LOGS.debug(StartEndYear +" : Year is selected...");
			
			Select month = new Select(getElementByClassAttr("ProjectCreateNew_monthDropDown_Class"));
			month.selectByVisibleText(StartEndMonth);
			APP_LOGS.debug(StartEndMonth +" : Month is selected...");
			
			WebElement datepicker= getObject("ProjectCreateNew_dateTable");
			//List<WebElement> rows = datepicker.findElements(By.tagName("tr"));
			List<WebElement> cols = datepicker.findElements(By.tagName("td"));
			for(WebElement cell :cols)
			{
				if(cell.getText().equals(StartEndDate))
				{
					cell.findElement(By.linkText(""+StartEndDate+"")).click();
					APP_LOGS.debug(StartEndDate +" : Date is selected...");
					break;
				}
			}
		}
		catch(Throwable t)
		{
			fail=true;
			t.printStackTrace();
		}
		
	}
	
	//Function to perform the selection of  version lead from the people  picker 
	public void toSelectVersionLeadfromPeoplePicker( WebElement VersionLeadPeoplePickerImage,String InputedTestData)
	{
		try{
			//Clicking on people picker image icon
			APP_LOGS.debug("Clicking on Version Lead People Picker Image Icon...");
			VersionLeadPeoplePickerImage.click();
	
			//Switching to the people picker frame 
			Thread.sleep(500);driver.switchTo().frame(1);
			APP_LOGS.debug("Switched to the Version Lead People Picker frame...");
			
			//Wait till the find text field is visible
			//wait = new WebDriverWait(driver,20);
			wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//input[@id='ctl00_PlaceHolderDialogBodySection_ctl07_queryTextBox']")));
	
			//Inputting test data in people picker text field
			getObject("ProjectCreateNew_versionLeadStakeholderTextField").sendKeys(InputedTestData); 
			APP_LOGS.debug(InputedTestData + " : Inputted text in Version Lead text field...");
		
			//Clicking on Search button 
			getObject("ProjectCreateNew_versionLeadStakeholderSearchBtn").click();
			APP_LOGS.debug("Clicked on Search button from People Picker ...");
		
			//Selecting search result 
			getObject("ProjectCreateNew_versionLeadStakeholderSelectSearchResult").click();
			APP_LOGS.debug("Version Lead is selected from searched user...");
		
			//Clicking on OK button from People Picker
			getObject("ProjectCreateNew_versionLeadStakeholderOkBtn").click();
			APP_LOGS.debug("Clicked on OK button from Version Lead People Picker frame ...");
		
			//Switching back to the default content 
			driver.switchTo().defaultContent();
			APP_LOGS.debug("Out of the Version Lead People Picker frame...");
	
		}
		catch(Throwable t){
			fail=true;
			t.printStackTrace();
		}
	}
	
	//Function to perform the selection of  version lead from the people  picker 
	public void toSelectStakeholderfromPeoplePicker( WebElement StakeholderPeoplePickerImage,String InputedTestData)
	{
			try{
				//Clicking on people picker image icon
				APP_LOGS.debug("Clicking on Version Stakeholder People Picker Image Icon...");		
				StakeholderPeoplePickerImage.click();
			
				//Switching to the people picker frame 
				Thread.sleep(500);driver.switchTo().frame(1);
				APP_LOGS.debug("Switched to the Version Stakeholder People Picker frame...");
				
				//Wait till the find text field is visible
				//wait = new WebDriverWait(driver,20);
				wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath("//input[@id='ctl00_PlaceHolderDialogBodySection_ctl07_queryTextBox']")));
				
				//Inputting test data in people picker text field
				getObject("ProjectCreateNew_versionLeadStakeholderTextField").sendKeys(InputedTestData); 
				APP_LOGS.debug(InputedTestData + " : Inputted text in Version Stakeholder text field...");
				
				//Clicking on Search button 
				getObject("ProjectCreateNew_versionLeadStakeholderSearchBtn").click();
				APP_LOGS.debug("Clicked on Search button from Version Stakeholder People Picker ...");
			
				//Selecting search result 
				getObject("ProjectCreateNew_versionLeadStakeholderSelectSearchResult").click();
				APP_LOGS.debug("Version Stakeholder is selected from searched user...");
				
				//Clicking on add button from Version Stakeholder people picker frame
				getObject("ProjectCreateNew_StakeholderAddBtn").click();
				APP_LOGS.debug("Clicked on Add button from Version Stakeholder People Picker frame ...");
						
				
				//Clicking on OK button from People Picker
				getObject("ProjectCreateNew_versionLeadStakeholderOkBtn").click();
				APP_LOGS.debug("Clicked on OK button from Version Stakeholder People Picker frame ...");
			
				//Switching back to the default content 
				driver.switchTo().defaultContent();
				APP_LOGS.debug("Out of the Version Stakeholder People Picker frame...");
			}
			catch(Throwable t)
			{
				fail=true;
				t.printStackTrace();
			}
		}
	
	public boolean validationMessageExceptVersionLead(WebElement SaveUpdateBtn,  String expectedValidationText, WebElement actualValidationExtractID)
	{
		try
		{
			//Checking for blank fields message for entering data in mandatory fields		
			SaveUpdateBtn.click();
			APP_LOGS.debug("Click on Save or Update button to check the validation message");
		
			//Verified the text displaying on pop up is same as that of the expected string 
			String actualValidatonText=actualValidationExtractID.getText();
			APP_LOGS.debug("Actual Message is : " +actualValidatonText+". And expected is : "+expectedValidationText);
			
			//Comparing text of validation messages 
			if (compareStrings(expectedValidationText, actualValidatonText))
			{
				APP_LOGS.debug("Assertion Successful.");
				Thread.sleep(800);
				getObject("ProjectCreateNew_validationPopUpOkButton").click();
				return true;
			}
			else
			{
				APP_LOGS.debug("Assertion UnSuccessful.");
				Thread.sleep(800);
				getObject("ProjectCreateNew_validationPopUpOkButton").click();
				assertTrue(false);
				return false;
			}			
			
			//Click on OK button form validation pop up message
			
		}
		catch(Throwable t)
		{
			comments+="Exception in 'validationMessageExceptVersionLead()'. ";
			t.printStackTrace();
			assertTrue(false);
			return false;
		}
	}
	
	public boolean validationForVersionLeadAndProjectSuccess(WebElement SaveBtn,  String expectedValidationText) throws InterruptedException
	{
		try
		{
			//Checking for blank fields message for entering data in manadatory fields		
			Thread.sleep(500);
			SaveBtn.click();
			APP_LOGS.debug("Click on Save button to check the validation message");
			
			//Verified the text dispalying on pop up is same as that of the expected string 
			String actualValidatonText;
			
			if (expectedValidationText.contains("successfully"))
				actualValidatonText = getTextFromAutoHidePopUp();
			else
				actualValidatonText=getElement("ProjectCreateNew_alertDiv_Id").getText();
			
			APP_LOGS.debug("Actual Message is : " +actualValidatonText+". And expected is : "+expectedValidationText);
		
			//Compairing the texts of validation message coming on keeping blank the Version Lead
			
			if (compareStrings(expectedValidationText, actualValidatonText))
			{
				APP_LOGS.debug("Assertion Successful.");
				Thread.sleep(800);
				
				if(!expectedValidationText.contains("successfully"))
					getObject("ProjectCreateNew_projectSuccessPopUpOkBtn").click();
				
				return true;
			}
			else
			{
				APP_LOGS.debug("Assertion UnSuccessful.");
				Thread.sleep(800);
				
				if(!expectedValidationText.contains("successfully"))
					getObject("ProjectCreateNew_projectSuccessPopUpOkBtn").click();
				
				assertTrue(false);
				return false;
			}	
			//Click on OK button form validation pop up message		
			
		}
		catch(Throwable t)
		{
			comments+="Exception in 'validationForVersionLeadAndProjectSuccess()'. ";
			t.printStackTrace();
			assertTrue(false);
			return false;
		}
	}
}
