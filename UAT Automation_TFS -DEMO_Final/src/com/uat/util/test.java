package com.uat.util;

import org.testng.annotations.Test;

import com.uat.base.TestBaseOld;

public class test extends TestBaseOld{

	@Test
	public void test1() throws Exception
	{
		initialize();
		openBrowser();
		login("Admin");
		getObject("testingSplit");
		closeBrowser();
	}
}
