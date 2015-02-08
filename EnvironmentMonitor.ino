#include <Wire\Wire.h>
#include <LiquidCrystal_I2C\LiquidCrystal_I2C.h>
#include <DHT\dht.h>

#define DHT_PIN 38
#define LED_PIN 13
#define CDS_PIN A0

LiquidCrystal_I2C LCD(0x27, 16, 2);
dht dht11;

bool activateLED = false;

void setup()
{
	/* initialize serial port */
	Serial.begin(115200);
	
	/* initialize LCD */
	LCD.init();
	LCD.backlight();

	String line1 = "T = 00"+ (String)(char)0xDF + "C H = 00%";
	String line2 = "Bright = 000.00%";

	LCD.print(line1);
	LCD.setCursor(0, 1);
	LCD.print(line2);

	/* initialize LED */
	pinMode(LED_PIN, OUTPUT);
	activateLED = true;
}

void loop()
{
	/*********************/
	/*     Get Value     */
	/*********************/

	//DHT11
	float temp, humid;
	if (dht11.read11(DHT_PIN) == DHTLIB_OK)
	{
		temp = dht11.temperature;
		humid = dht11.humidity;
	}

	//Brightness
	int bright_raw = analogRead(CDS_PIN);
	float bright = (bright_raw > 800) ? 100 : ((bright_raw < 250) ? 0 : ((bright_raw - 250) / 5.5));

	// GetLED
	if (Serial.available())
	{
		switch(Serial.read())
		{
			case '1':
				activateLED = true;
				break;
			case '0':
				activateLED = false;
			default:
				break;
		}
	}


	// Adjust LED brightness
	if (activateLED)
	{
		byte led_bright = map(bright, 0, 100, 0, 255);
		led_bright = 255 - led_bright;
		analogWrite(LED_PIN, led_bright);
	}
	else
	{
		analogWrite(LED_PIN, 0);
	}


	/*********************/
	/*    Display LCD    */
	/*********************/

	// Temperature
	LCD.setCursor(4, 0);
	LCD.print(temp,0);

	// Humidity
	LCD.setCursor(13, 0);
	LCD.print(humid, 0);
	
	//Brightness
	LCD.setCursor(9, 1);
	if (bright == 100)
	{ }
	else if (bright < 10)
	{
		LCD.print("  ");
	}
	else
	{
		LCD.print(" ");
	}
	LCD.print(bright);

	/*********************/
	/*   Display serial  */
	/*********************/

	//Temperature and Himudity
	String str = (String)temp + "/" + (String)humid + "/" + (String)bright;
	Serial.println(str);


	/*********************/
	/*       Delay       */
	/*********************/
	delay(200);
	
}
