#Escalonador

def findWaitingTime(processes, n, 
                    bursttime, waitingtime): #tempo para todos os processos
      
    waitingtime[0] = 0 #tempo de espera do primeiro processo é 0
    
    for i in range(1, n ): 
        waitingtime[i] = bursttime[i - 1] + waitingtime[i - 1]  #calculando tempo de espera
  
def findTurnAroundTime(processes, n,  
                       bursttime, waitingtime, tat): #função para troca de processo
    
    for i in range(n): 
        tat[i] = bursttime[i] + waitingtime[i] # calculando tempo de troca com bursttime[i] + waitingtime[i] 

def findavgTime( processes, n, bursttime): #Função para calcular tempo médio
  
    waitingtime = [0] * n 
    tat = [0] * n  
    totalwait = 0
    total_tat = 0
    
    findWaitingTime(processes, n, bursttime, waitingtime) #Função apra calcular tempo de espera de todos os processos 
    
    findTurnAroundTime(processes, n,  
                       bursttime, waitingtime, tat) #Função para localizar tempo de troca em todos os processos
    
    print( "Tempo de burst " + 
                  " Tempo de espera " + 
                " Tempo de troca") #Mostrar processos com todos os detalhes
    
    for i in range(n): #Calcula tempo total de espera e tempo total de troca
        totalwait = totalwait + waitingtime[i] 
        total_tat = total_tat + tat[i] 
        print(" " + str(i + 1) + "\t\t" + 
                    str(bursttime[i]) + "\t " + 
                    str(waitingtime[i]) + "\t\t " + 
                    str(tat[i]))  
  
    print( "Tempo médio de espera = "+
                   str(totalwait / n)) 
    print("Tempo médio de troca = "+
                     str(total_tat / n)) 
  
if __name__ =="__main__": 
    processes = [ 1, 2, 3, 4, 5, 6, 7, 8] #ID de cada processo
    n = len(processes)

    burst_time = [12, 5, 8, 5, 17, 4, 1, 15] #Tempo de burst de todos os processos
    findavgTime(processes, n, burst_time)
