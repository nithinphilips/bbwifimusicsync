/*
 * This Script tests all possible options.
 *
 */
 
 
// Simple script
scan "http://www.example.com/example.html" with filter ".oneFile" extract links save to "C:\Downloads"

// Nested script
scan "http://www.example.com/example.html" with filter ".oneFile" extract links
	scan results with filter "link" extract links
		scan results with filter "jpg" extract links and resources
			scan results with filter "jpg" extract resources save to "C:\Downloads" rename "{name}.{ext}"

// Another nested script with minimal arguments
scan "http://www.example.com/example.html" save to "C:\Downloads"