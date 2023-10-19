using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ObserverPattern
{
    //public interface IObserver<T1>
    //{
    //    void Notify(T1 t1);
    //}

    //public interface IObserver<T1, T2>
    //{
    //    void Notify(T1 t1, T2 t2);
    //}

    //public interface IObserver<T1, T2, T3>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3);
    //}

    //public interface IObserver<T1, T2, T3, T4>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4);
    //}

    //public interface IObserver<T1, T2, T3, T4, T5>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    //}

    //public interface IObserver<T1, T2, T3, T4, T5, T6>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    //}

    //public interface IObserver<T1, T2, T3, T4, T5, T6, T7>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    //}

    //public interface IObserver<T1, T2, T3, T4, T5, T6, T7, T8>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
    //}

    //public interface IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
    //}

    //public interface IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    //{
    //    void Notify(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10);
    //}

    //public interface ISubject<T1>
    //{
    //    public Action<T1> OnEventRequested { get; set; }

    //    void AddObserver(IObserver<T1> observer);

    //    void RemoveObserver(IObserver<T1> observer);

    //    void NotifyToObservers(T1 t1);
    //}

    //public interface ISubject<T1, T2>
    //{
    //    public Action<T1, T2> OnEventRequested { get; set; }

    //    void AddObserver(IObserver<T1, T2> observer);

    //    void RemoveObserver(IObserver<T1, T2> observer);

    //    void NotifyToObservers(T1 t1, T2 t2);
    //}

    //public interface ISubject<T1, T2, T3>
    //{
    //    List<IObserver<T1, T2, T3>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3);
    //}

    //public interface ISubject<T1, T2, T3, T4>
    //{
    //    List<IObserver<T1, T2, T3, T4>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4);
    //}

    //public interface ISubject<T1, T2, T3, T4, T5>
    //{
    //    List<IObserver<T1, T2, T3, T4, T5>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4, T5> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4, T5> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    //}

    //public interface ISubject<T1, T2, T3, T4, T5, T6>
    //{
    //    List<IObserver<T1, T2, T3, T4, T5, T6>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4, T5, T6> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4, T5, T6> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    //}

    //public interface ISubject<T1, T2, T3, T4, T5, T6, T7>
    //{
    //    List<IObserver<T1, T2, T3, T4, T5, T6, T7>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4, T5, T6, T7> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4, T5, T6, T7> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    //}

    //public interface ISubject<T1, T2, T3, T4, T5, T6, T7, T8>
    //{
    //    List<IObserver<T1, T2, T3, T4, T5, T6, T7, T8>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4, T5, T6, T7, T8> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4, T5, T6, T7, T8> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
    //}

    //public interface ISubject<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    //{
    //    List<IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
    //}

    //public interface ISubject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    //{
    //    List<IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> Observers { get; set; }

    //    void AddObserver(IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> observer);

    //    void RemoveObserver(IObserver<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> observer);

    //    void NotifyToObservers(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10);
    //}
}