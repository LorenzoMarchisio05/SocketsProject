# SocketsProject

Il progetto in se è diviso in vari progetti troviamo nella solution 4 principali cartelle ( Server, client, weather.commons, e weather.graph ).
Weather.commons è una libreria di classi con una serie di oggetti/implementazioni condivisi sui vari progetti (client server e graph).
IL db è dentro la cartella DB allo stesso livello della solution ( il path viene preso a compile time dal progetto ).
in bin debug del server è presente una cartella rilevazioni con un file di log contenente i dati del server in formato csv.

Weather.Graph contiene un form con una serie di grafici con andamento di temperature e umidità per ognuno delle varie stazione.

Server e Client sfruttano un simil patter della Clean Architecture (**https://betterprogramming.pub/the-clean-architecture-beginners-guide-e4b7058c1165**)
dove il codice è diviso in Entity per le entità semplici, Infrastructure per le implementezioni effettive, 
View per le effettuve implementazioni grafiche (nel mio caso 2 _console e _winform).

Il progetto è totalmente indipendente dalla view, che sia un form, una console o una web app, sfrutta eventi per "dialogare" con la view.
Il server è un sql server locale, per accederci utilizzo i metodi AdoNet con una mia personale implementazione di un controller (un wrapper dei metodi adonet).
