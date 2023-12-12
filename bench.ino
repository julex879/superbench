 #define SER 7    // pin 7 a SER del 74HC595
#define RCLK 8    // pin 8 a RCLK del 74HC595
#define SRCLK 9   // pin 9 a SRCLK del 74HC595

#define LED0 1    // valor equivalente en decimal al LED0
#define LED1 2    // valor equivalente en decimal al LED1
#define LED2 4    // valor equivalente en decimal al LED2
#define LED3 8    // valor equivalente en decimal al LED3
#define LED4 16   // valor equivalente en decimal al LED4
#define LED5 32   // valor equivalente en decimal al LED5
#define LED6 64   // valor equivalente en decimal al LED6
#define LED7 128  // valor equivalente en decimal al LED7
#define LED8 256  // valor equivalente en decimal al LED8
#define LED9 512  // valor equivalente en decimal al LED9
#define LED10 1024  // valor equivalente en decimal al LED10
#define LED11 2048  // valor equivalente en decimal al LED11
#define LED12 4096  // valor equivalente en decimal al LED12
#define LED13 8192  // valor equivalente en decimal al LED13
#define LED14 16384 // valor equivalente en decimal al LED14
#define LED15 32768 // valor equivalente en decimal al LED15
uint32_t dato;
String inputBuffer = "";
union DatosUnion {
  uint32_t valorCompleto=0x00000000;
  struct {
    uint8_t byte1;
    uint8_t byte2;
    uint8_t byte3;
    uint8_t byte4;
  };
}; 
  DatosUnion datos;

char  basura;
volatile bool datosListos = false;

const int pinVoltaje = A0;
const int pinCorriente = A7;
const float factorCalibracion = 0.066;  // Ajustar según la especificación del sensor

unsigned long previousMillis = 0;  // Variable para almacenar el tiempo anterior
const int delayTime = 200;        // Tiempo de delay en milisegundos

void setup ()
{
  Serial.begin(115200);
  pinMode(SER, OUTPUT);   // pin establecido como salida
  pinMode(RCLK, OUTPUT);  // pin establecido como salida
  pinMode(SRCLK, OUTPUT); // pin establecido como salida
  Funcion_De_Comunication();

}
 
void loop() {
  
  unsigned long currentMillis = millis();

  // Verificar si ha pasado el tiempo especificado
  if (currentMillis - previousMillis >= delayTime) {
    // Actualizar el tiempo anterior
    previousMillis = currentMillis;  
  float voltajePromedio = obtenerPromedioVoltaje(10);  // Promedio de 10 lecturas
  float corrientePromedio = obtenerPromedioCorriente(10);  // Promedio de 10 lecturas

  // Imprimir los valores con mayor precisión
  
  Serial.print(voltajePromedio, 3);  // Tres decimales
  Serial.print(" ");
  Serial.println(corrientePromedio, 3);  // Tres decimales



    }
}
float obtenerPromedioVoltaje(int numLecturas) {
  float sumaVoltaje = 0.0;

  for (int i = 0; i < numLecturas; ++i) {
    sumaVoltaje += (float)analogRead(pinVoltaje) * (13.1 / 1023.0);
    delay(10);  // Pequeño retardo entre lecturas
  }

  return sumaVoltaje / numLecturas;
}

float obtenerPromedioCorriente(int numLecturas) {
  float sumaCorriente = 0.0;

  for (int i = 0; i < numLecturas; ++i) {
    sumaCorriente += fabs((analogRead(pinCorriente) - 512.0) * (5.0 / 1024.0) / factorCalibracion);
    delay(10);  // Pequeño retardo entre lecturas
  }

  return sumaCorriente / numLecturas;
}

void serialEvent() {
  while (Serial.available()) {
    // Lee el caracter del puerto serie
    char inChar = (char)Serial.read();

    // Si el caracter es un salto de línea, procesa la cadena
    if (inChar == '\n') {
      procesarCadena(inputBuffer);

 
       Funcion_De_Comunication();
      inputBuffer = ""; // Reinicia el buffer para la próxima cadena
    } else {
      // Agrega el caracter al buffer
      inputBuffer += inChar;
    }
  }
}


void procesarCadena(String cadena) {
  // Buscar la posición del guion ("-")
  int guionIndex = cadena.indexOf('-');

  // Verificar si se encontró el guion
  if (guionIndex != -1) {
    // Extraer la parte de nivel
    String nivelStr = cadena.substring(0, guionIndex);
    // Extraer la parte de valor
    String valorStr = cadena.substring(guionIndex + 1);

    int nivel = nivelStr.toInt();
    int valor = valorStr.toInt();

    




    if(valor<32){
              if (nivel == 0) {
                  // Limpiar el bit en la posición especificada por el valor
                  datos.valorCompleto &= ~(1UL<< valor);
                }
                else if (nivel == 1) {
                  // Establecer el bit en la posición especificada por el valor
                  datos.valorCompleto |= (1UL << valor);
                }
    }else{
        switch (valor) {

            case 32: 
                datos.valorCompleto=0x00000000;
            break;
            case 33:

                
                datos.valorCompleto &= 0xF0000000;
            break;

          default:
         
            // Código para el caso por defecto (cuando la opción no coincide con ninguno de los casos anteriores)
          break;
          
          
          
          }


    }


    
                   } 
  
  else {
   
  }
}




void Funcion_De_Comunication( ){
    digitalWrite(RCLK, LOW);        // nivel bajo a registro de almacenamiento
      shiftOut(SER, SRCLK, MSBFIRST,~datos.byte4 );
      shiftOut(SER, SRCLK, MSBFIRST,~datos.byte3 );
      shiftOut(SER, SRCLK, MSBFIRST,~datos.byte2 );
      shiftOut(SER, SRCLK, MSBFIRST,~datos.byte1 );
    digitalWrite(RCLK, HIGH);       // nivel alto a registro de almacenamiento
}
void mover_todos(){
      datos.valorCompleto=0x00000001;
      for(int i=0;i<32;i++){

       Serial.println(datos.valorCompleto,HEX);
       Serial.println(datos.valorCompleto,BIN);
       Serial.print("bit: ");
     
        Serial.print(datos.byte1,BIN);
        Serial.print(" ");  
        Serial.print(datos.byte2,BIN);
        Serial.print(" ");  
        Serial.print(datos.byte3,BIN);
        Serial.print(" ");  
        Serial.println(datos.byte4,BIN);
        Serial.println();

        
        Funcion_De_Comunication();
        delay(500);
      datos.valorCompleto<<=1;
      }  
}


   
