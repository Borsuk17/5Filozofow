using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace _5Filozofów1
{
   
    class PieciuFilozofow
    {
        static void Main(string[] args)
        {
            PieciuFilozofow pf = new PieciuFilozofow();

            pf.Startuj();
            Thread.Sleep(20000); //czekamy losowy czas
            pf.Zakoncz(); //przerywamy wątki
        }

        private Semaphore[] widelce;  //tablica binarnych semaforów widelce
        private Semaphore lokaj;  //semafor lokaj, który dopuszcza maksymalnie 4 filozofów do stołu
        private Thread[] filozofowie; //tablica wątków filozofów
        
        // Konstruktor klasy PieciuFilozofow odpowiedzialny między innymi
        // za inicjalizację i nadanie nazw wątkom.
        
        public PieciuFilozofow()
        {
            widelce = new Semaphore[5];
            for (int i = 0; i < 5; i++)
            {
                widelce[i] = new Semaphore(1, 1);  //semafory binarne
            }
            lokaj = new Semaphore(4, 4);
          
            filozofowie = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                filozofowie[i] = new Thread(new ThreadStart(Filozof)); //tworzymy wątki
                filozofowie[i].Name = "Filozof " + i; //i nadajemy im nazwy
            }

        }

        public void Startuj()  /// Funkcja uruchamiająca wszystkie wątki.
        {
            foreach (Thread t in filozofowie)
            {
                t.Start();
            }
        }
        
        public void Zakoncz()  /// Funkcja przerywająca wszystkie wątki
        {
            foreach (Thread t in filozofowie)
            {
                t.Interrupt();
            }
        }
        
        /// Wątek filozofa, który w nieskończonej pętli myśli przez losowy czas,
        /// a następnie próbuje zdobyć dwa widelce i przez losowy czas je.
        public void Filozof()
        {
            try
            {
                int numer = Int32.Parse(Thread.CurrentThread.Name.Split(' ')[1]);
                Random rand = new Random();
                while (true)
                {
                    //Console.WriteLine(Thread.CurrentThread.Name.Split(' ')[1]);
                    Console.WriteLine("{0} myśli", Thread.CurrentThread.Name);
                    Thread.Sleep(rand.Next(40, 800)); //losowy czas myślenia

                    lokaj.WaitOne(); //sprawdzamy, czy możemy usiąść do stołu
                    widelce[numer].WaitOne(); //bierzemy lewy widelec
                    widelce[(numer + 1) % 5].WaitOne(); //bierzemy prawy widelec
                    Console.WriteLine("{0} je", Thread.CurrentThread.Name );
                    //Console.WriteLine(Thread.CurrentThread.Name + "  je ");
                    Thread.Sleep(rand.Next(100, 300)); //losowy czas jedzenia
                    widelce[numer].Release(); //odkładamy lewy widelec
                    widelce[(numer + 1) % 5].Release(); //odkładamy prawy widelec
                    lokaj.Release(); //dopuszczamy kolejnego filozofa do stołu                    
                }
            }
            catch (ThreadInterruptedException)
            { }
        }
    }
}
