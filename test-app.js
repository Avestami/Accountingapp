const puppeteer = require('puppeteer');

async function testApplication() {
    console.log('Starting application test...');
    
    const browser = await puppeteer.launch({
        headless: false, // Set to true for headless mode
        defaultViewport: null,
        args: ['--start-maximized']
    });

    try {
        const page = await browser.newPage();
        
        // Test if the application is running
        console.log('Testing application availability...');
        
        // Try to connect to the frontend
        try {
            await page.goto('http://localhost:3000', { waitUntil: 'networkidle2', timeout: 10000 });
            console.log('✅ Frontend is accessible at http://localhost:3000');
            
            // Take a screenshot
            await page.screenshot({ path: 'frontend-test.png', fullPage: true });
            console.log('📸 Frontend screenshot saved as frontend-test.png');
            
        } catch (error) {
            console.log('❌ Frontend not accessible at http://localhost:3000');
            console.log('Error:', error.message);
        }

        // Try to connect to the API
        try {
            await page.goto('http://localhost:5000/api/health', { waitUntil: 'networkidle2', timeout: 10000 });
            console.log('✅ API is accessible at http://localhost:5000');
            
            // Take a screenshot of API response
            await page.screenshot({ path: 'api-test.png' });
            console.log('📸 API screenshot saved as api-test.png');
            
        } catch (error) {
            console.log('❌ API not accessible at http://localhost:5000');
            console.log('Error:', error.message);
        }

        // Test basic navigation if frontend is available
        try {
            await page.goto('http://localhost:3000', { waitUntil: 'networkidle2' });
            
            // Wait for the page to load
            await page.waitForTimeout(2000);
            
            // Check if login form exists
            const loginForm = await page.$('form');
            if (loginForm) {
                console.log('✅ Login form found on homepage');
            }
            
            // Check for navigation elements
            const navElements = await page.$$('nav, .nav, [role="navigation"]');
            if (navElements.length > 0) {
                console.log('✅ Navigation elements found');
            }
            
        } catch (error) {
            console.log('❌ Error testing frontend navigation:', error.message);
        }

    } catch (error) {
        console.error('Test failed:', error);
    } finally {
        await browser.close();
        console.log('Test completed.');
    }
}

// Run the test
testApplication().catch(console.error);