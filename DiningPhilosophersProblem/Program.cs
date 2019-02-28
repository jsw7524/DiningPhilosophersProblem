using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophersProblem
{

    class Fork
    {
        private Mutex mutex;
        private static int numberOfForks = 0;
        public int FID;
        public Fork()
        {
            mutex = new Mutex(false);
            numberOfForks += 1;
            FID = numberOfForks;
        }

        public void GetFork()
        {
            mutex.WaitOne();
        }
        public void ReleaseFork()
        {
            mutex.ReleaseMutex();
        }
    }

    class Philosopher
    {
        enum States
        {
            Thinking,
            Eating
        }

        private static int numberOfPhilosophers = 0;
        public int PID;

        private Random rnd;

        private States state;
        private List<Fork> forksOnTable;

        private Fork rightFork;
        private Fork leftFork;

        public Philosopher(List<Fork> f)
        {
            PID = numberOfPhilosophers;
            numberOfPhilosophers += 1;
            rnd = new Random(7524 + PID);
            state = States.Thinking;
            forksOnTable = f;
        }

        public void DoSomething()
        {
            if (state == States.Thinking)
            {
                Console.WriteLine($"Philosopher {PID} is Thinking");
                if (rnd.Next(0, 10) > 2)
                {
                    state = States.Eating;
                }
            }
            else
            {
                //right fork
                rightFork = (PID == 0) ? forksOnTable[numberOfPhilosophers - 1] : forksOnTable[PID - 1];

                //left fork
                leftFork = forksOnTable[PID];

                rightFork.GetFork();
                Console.WriteLine($"Philosopher {PID}  Grabs right fork");

                leftFork.GetFork();
                Console.WriteLine($"Philosopher {PID}  Grabs left fork");

                Console.WriteLine($"Philosopher {PID} is Eating");

                rightFork.ReleaseFork();
                leftFork.ReleaseFork();

                state = States.Thinking;
            }
        }
    }

    class DiningRoom
    {
        private List<Fork> forksOnTable = new List<Fork>();

        async Task Execute()
        {
            await Task.Run(() =>
            {
                Philosopher p = new Philosopher(forksOnTable);

                while (true)
                {
                    p.DoSomething();
                }
            });
        }

        public void PlayDiningPhilosophersProblem()
        {
            int size = 5;

            for (int i = 0; i < size; i++)
            {
                forksOnTable.Add(new Fork());
            }
            for (int i = 0; i < size; i++)
            {
                Execute();
            }

        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            DiningRoom diningRoom = new DiningRoom();
            diningRoom.PlayDiningPhilosophersProblem();
            Console.ReadLine();
        }
    }
}
