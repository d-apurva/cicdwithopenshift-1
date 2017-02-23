package com.uat.util;

import java.io.File;

import org.apache.commons.io.FileUtils;
import org.openqa.selenium.OutputType;
import org.openqa.selenium.TakesScreenshot;

import com.uat.base.TestBaseOld;

public class TestUtil extends TestBaseOld{

	
	// finds if the test suite is runnable 
		public static boolean isSuiteRunnable(Xls_Reader xls , String suiteName){
			boolean isExecutable=false;
			for(int i=2; i <= xls.getRowCount("Test Suite") ;i++ ){
				//String suite = xls.getCellData("Test Suite", "TSID", i);
				//String runmode = xls.getCellData("Test Suite", "Runmode", i);
			
				if(xls.getCellData("Test Suite", "TSID", i).equalsIgnoreCase(suiteName)){
					if(xls.getCellData("Test Suite", "Runmode", i).equalsIgnoreCase("Y")){
						isExecutable=true;
					}else{
						isExecutable=false;
					}
				}

			}
			xls=null; // release memory
			return isExecutable;
			
		}
		
		
		// returns true if runmode of the test is equal to Y
		public static boolean isTestCaseRunnable(Xls_Reader xls, String testCaseName){
			boolean isExecutable=false;
			for(int i=2; i<= xls.getRowCount("Test Cases") ; i++){
				//String tcid=xls.getCellData("Test Cases", "TCID", i);
				//String runmode=xls.getCellData("Test Cases", "Runmode", i);
				//System.out.println(tcid +" -- "+ runmode);
				
				
				if(xls.getCellData("Test Cases", "TCID", i).equalsIgnoreCase(testCaseName)){
					if(xls.getCellData("Test Cases", "Runmode", i).equalsIgnoreCase("Y")){
						isExecutable= true;
					}else{
						isExecutable= false;
					}
				}
			}
			
			return isExecutable;
			
		}
		
		
		// return the test data from a test in a 2 dim array
		public static Object[][] getData(Xls_Reader xls , String testCaseName){
			// if the sheet is not present
			if(! xls.isSheetExist(testCaseName)){
				xls=null;
				return new Object[1][0];
			}
			
			
			int rows=xls.getRowCount(testCaseName);
			int cols=xls.getColumnCount(testCaseName);
			//System.out.println("Rows are -- "+ rows);
			//System.out.println("Cols are -- "+ cols);
			
		    Object[][] data =new Object[rows-1][cols-3];
			for(int rowNum=2;rowNum<=rows;rowNum++){
				for(int colNum=0;colNum<cols-3;colNum++){
					//System.out.print(xls.getCellData(testCaseName, colNum, rowNum) + " -- ");
					data[rowNum-2][colNum] = xls.getCellData(testCaseName, colNum, rowNum);
				}
				//System.out.println();
			}
			return data;
			
		}
		
		
		
		// checks RUnmode for dataSet
		public static String[] getDataSetRunmodes(Xls_Reader xlsFile,String sheetName){
			String[] runmodes=null;
			if(!xlsFile.isSheetExist(sheetName)){
				xlsFile=null;
				sheetName=null;
				runmodes = new String[1];
				runmodes[0]="Y";
				xlsFile=null;
				sheetName=null;
				return runmodes;
			}
			runmodes = new String[xlsFile.getRowCount(sheetName)-1];
			for(int i=2;i<=runmodes.length+1;i++){
				runmodes[i-2]=xlsFile.getCellData(sheetName, "Runmode", i);
			}
			xlsFile=null;
			sheetName=null;
			return runmodes;
			
		}

		// update results for a particular data set	
		public static void reportDataSetResult(Xls_Reader xls, String testCaseName, int rowNum,String result){	
			xls.setCellData(testCaseName, "Result", rowNum, result);
		}
		
		public static void printComments(Xls_Reader xls, String testCaseName, int rowNum,String comments) {
			   xls.setCellData(testCaseName, "Comments", rowNum, comments);
			  }
		
		// return the row num for a test
		public static int getRowNum(Xls_Reader xls, String id){
			for(int i=2; i<= xls.getRowCount("Test Cases") ; i++){
				String tcid=xls.getCellData("Test Cases", "TCID", i);
				
				if(tcid.equals(id)){
					xls=null;
					return i;
				}
				
				
			}
			
			return -1;
		}
		
		
		
		public static void takeScreenShot(String testCaseName, String fileName) 
		{
			File scrFile = ((TakesScreenshot)driver).getScreenshotAs(OutputType.FILE);
			
			try 
		    {
				FileUtils.copyFile(scrFile, new File(System.getProperty("user.dir")+"\\screenshots\\"+screenshotDateTime+"\\"+testCaseName+"\\"+fileName+".jpg"));
		    } 
			catch (Exception e) 
			{		   
				e.printStackTrace();
			}
		}
		
	/*	public static void UserLoginWithRole(Xls_Reader xls ,String Role) throws Exception
		{
			Object actualRole;
			for(int i =2 ; i<=xls.getRowCount("Roles For Login") ; i++)
			{
				String username = xls.getCellData("Roles For Login", "Username", i);
				String role =xls.getCellData("Roles For Login", "Role", i);
				String password = xls.getCellData("Roles For Login", "Password", i);
				
				if(role.equalsIgnoreCase(Role) )
				{
					System.out.println("Loggin with user : " + username + " having role as : " + role);
					login(username,password);
					Thread.sleep(3000);
					actualRole = (Object) eventfiringdriver.executeScript("return security.userType");
					System.out.println(actualRole);
					String s1 = actualRole.toString();
					System.out.println(s1);
					if(actualRole.toString().contains("3")){
						System.out.println("Role is correct");
						return;
						
					}
					return;
						
				}
			}
		}*/
		
		
		
		
		
		
		
		
		
		
		
		
		
}
