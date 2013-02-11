//+------------------------------------------------------------------+
//|                                       AutoFXProfits - Sender.mq4 |
//|                                  Copyright 2013, Auto FX Profits |
//+------------------------------------------------------------------+

#property copyright "Copyright 2013, Auto FX Profits"

#import "Communication Library Demo.dll"
   bool     SendToSocket(string orderInfo);
   bool     SpawnClientTerminal(int handle, string terminalName);
#import

#include <stdlib.mqh>

//=============GLOBAL VARIABLES=============
//string PredefinedPrefix=".,m,fx,_fx,$,pro,r,FXF,.arm,-,_,.,iam,SB,fxr,v";
string PredefinedPrefix = ".,m,fx,_fx,$,FXF,pro,.arm,-,v,fxr,SB,iam,2,r,_,fxr";       //Defined set of Symbol Prefixes
string PredefinedPostfix = ".,m,fx,_fx,$,FXF,pro,.arm,-,v,fxr,SB,iam,2,r,_,fxr";      //Defined set of Symbol Postfixes

string SymbolPrefix  = "";                      //The symbol prefix for the system
string SymbolPostfix = "";                      //The symbol postfix for the system

double RiskPercent = 1;

static int      Token;
static int      NumberOfOrders = 0;
static int      LastProcessingTk;

static int      TicketArr    [100];
static int      TypeArr      [100];
static double   OpenPriceArr [100];
static double   LotSizeArr   [100];
static double   RiskPercentArr   [100];
static double   StopLossArr  [100];
static double   TakeProfitArr[100];

static string   PartialCloseTickets[100];
static int      PartialCloseCount = 0;

int      temp_TicketArr[100];
int      temp_TypeArr[100];
double   temp_OpenPriceArr[100];
double   temp_LotSizeArr[100];
double   temp_RiskPercentArr[100];
double   temp_StopLossArr[100];
double   temp_TakeProfitArr[100];

//+------------------------------------------------------------------+
//| expert initialization function                                   |
//+------------------------------------------------------------------+
int init()
  {
//----
   if (AccountNumber() == 0)
   {
      return (0);
   }
   
   if (!IsDllsAllowed())
   {
      Comment("Please Allow DLL Import!");
      Print("[init]Please Allow DLL Import!");
      return (0);
   }
   
   string mode = "";
   if(IsTesting())
   {
      mode = "TEST";
   }
   else
   {
      mode = "LIVE";
   }
   
   SpawnClientTerminal(WindowHandle(Symbol(), Period()), "datasource");
   
   SetSymbolPrefixAndPostfix();
   
   if (!AuthenticateSender())
   {
      Alert("TradeMirror Sender: NO PERMISION TO POST SIGNALS!!!");
      Print("[init]TradeMirror Sender: NO PERMISION TO POST SIGNALS!!!");
      Token=0;
   }
   else
   {
      Alert("TradeMirror Sender is READY!");
      Print("[init]TradeMirror Sender is READY!");
      Token=1;
   }
   
   if (Token==1)
   {
      int total = OrdersTotal();
      
      for(int pos = total - 1; pos >= 0; pos--)
      {    
         if(OrderSelect(pos, SELECT_BY_POS))
         {
            TicketArr[pos] = OrderTicket();
            TypeArr[pos] = OrderType();
            LotSizeArr[pos] = OrderLots();
            RiskPercentArr[pos] = RiskPercent;
            OpenPriceArr[pos] = OrderOpenPrice();
            StopLossArr[pos] = OrderStopLoss();
            TakeProfitArr[pos] = OrderTakeProfit();
         }
      }
      NumberOfOrders = total;
   }
   
   LastProcessingTk = 0;
//----
   return(0);
  }
//+------------------------------------------------------------------+
//| expert deinitialization function                                 |
//+------------------------------------------------------------------+
int deinit()
  {
//----
   
//----
   return(0);
  }
//+------------------------------------------------------------------+
//| expert start function                                            |
//+------------------------------------------------------------------+
int start()
  {
   if (Token==1)
   {
      while(!IsStopped() && IsExpertEnabled())
      {
         SendSignals();
         Sleep(100);
      }
   }
   
   return(0);
  }
//+------------------------------------------------------------------+

//+-------------------------------------------------------------------------------------+
//|                               START Utility                                         |
//+-------------------------------------------------------------------------------------+
void SetSymbolPrefixAndPostfix()
{
   /*if (MarketInfo("EURUSD", MODE_POINT) != 0)      //If there is not prefix & postfix in our symbol
   {
      SymbolPrefix = "";
      SymbolPostfix = "";
      Print("[SetSymbolPrefixAndPostfix]No prefix/postfix needed");
   }
   else
   {*/
      string preList[17];
      string posList[17];
      
      int index = 0;

      int pos = 0;
      pos = StringFind(PredefinedPrefix , ",");

      preList[index] = StringSubstr(PredefinedPrefix, 0, pos);
      posList[index] = StringSubstr(PredefinedPrefix, 0, pos);
      //Print(subString[index]);
      index++;

      PredefinedPrefix = StringSubstr(PredefinedPrefix, pos+1);
      //Print(message);

      while(pos != -1)
      {
         pos = StringFind(PredefinedPrefix , ",");
         preList[index] = StringSubstr(PredefinedPrefix, 0, pos);
         posList[index] = StringSubstr(PredefinedPrefix, 0, pos);
         //Print(preList[index]);
         index++;
         PredefinedPrefix = StringSubstr(PredefinedPrefix, pos+1);
         //Print(PredefinedPrefix);
      }
      
      bool found = false;
      for (int i=0; i<ArraySize(posList); i++)
      {
         string testSymbol = "EURUSD" + posList[i];
         if (MarketInfo( testSymbol, MODE_POINT  )!=0)
         {
            SymbolPrefix = "";
            SymbolPostfix = posList[i];       
            found = true;
            Print("[SetSymbolPrefixAndPostfix]Symbol Postfix = " + SymbolPostfix);
            break;
         }
      }
      
      if (!found)
      {
         for (i=0; i<ArraySize(preList); i++)
         {
            testSymbol = preList[i] + "EURUSD";
            if (MarketInfo( testSymbol, MODE_POINT  )!=0)
            {
               SymbolPrefix = preList[i];
               SymbolPostfix = "";       
               found = true;
               Print("[SetSymbolPrefixAndPostfix]Symbol Prefix = " + SymbolPrefix);
               break;
            }
         }   
      }
      
      if (!found)
      {
         for (i=0; i<ArraySize(preList); i++)
         {
            for (int j=0; j<ArraySize(posList); j++)
            {
               testSymbol = preList[i] + "EURUSD" + posList[j];
               if (MarketInfo( testSymbol, MODE_POINT  )!=0)
               {
                  SymbolPrefix = preList[i];
                  SymbolPostfix = posList[j];
                  Print("[SetSymbolPrefixAndPostfix]Symbol Postfix = " + SymbolPostfix + " | Symbol Prefix = " + SymbolPrefix);
                  found = true;
                  break;
               }
            }
         }   
      }         
   //}
}


bool AuthenticateSender()
{
   //Authenticate the sender here
   return(true);
}
//+-------------------------------------------------------------------------------------+
//|                                    END Utility                                      |
//+-------------------------------------------------------------------------------------+


//+-------------------------------------------------------------------------------------+
//|                                  START Core Logic                                   |
//+-------------------------------------------------------------------------------------+
void SendSignals()
{
   //+------------------------------NEW ORDER PROCESSING----------------------------+
   int partialCloseNewTicket = -1;
   int partialCloseOldTicket = -1;
   
   int total = OrdersTotal();
   int orderArrays[1000];
   int orderCount=0; 
        
   for(int pos = total - 1; pos >= 0; pos--)
   {    
      if(OrderSelect(pos, SELECT_BY_POS))
      {
         orderArrays[orderCount] = OrderTicket();
         orderCount++;
      }
   }
   
   if (orderCount>0)
   {
      ArraySort(orderArrays, orderCount, 0, MODE_ASCEND);
   }
            
   for(int index = 0; index < orderCount; index++)
   {    
      if(OrderSelect(orderArrays[index], SELECT_BY_TICKET))
      {  
         //Print("Order Ticket = " +  OrderTicket() + " | LastProcessingTk = " + LastProcessingTk);
         if (OrderTicket() >= LastProcessingTk)                
         {                     
            int currentTicket = OrderTicket();
            int masterTicket = CheckPartialClose(OrderSymbol(), OrderType(), OrderLots(), OrderOpenPrice(), 
                                                 OrderOpenTime(), OrderStopLoss(), OrderTakeProfit(), OrderMagicNumber());
                                             
            if (masterTicket==0)
            {
               LastProcessingTk = currentTicket;   //already processed
               continue;               
            }
         
            if (masterTicket >0) 
            {         
               partialCloseOldTicket=masterTicket;
               partialCloseNewTicket=currentTicket;
               Print("[SendSignals]Partial Close Order #"+ partialCloseOldTicket + ", New Ticket #" + partialCloseNewTicket);
               break;                       
            }
            Print("[SendSignals]New Ordertime = ",TimeToStr(OrderOpenTime(),TIME_DATE|TIME_SECONDS));
         
            OrderSelect(currentTicket, SELECT_BY_TICKET);
         
            //Put the new order to array
            if (OrderMagicNumber() != 0 && OrderStopLoss() == 0)
            {
               int retry = 1;
               while (retry <= 10)
               {
                  Sleep(100);       //ECN mode? wait for stoploss is set.
                  OrderSelect(OrderTicket(), SELECT_BY_TICKET);
                  if (OrderStopLoss() > 0)
                  {
                     break;
                  }
                  retry++;
               }
            }

            TicketArr[NumberOfOrders] = OrderTicket();
            TypeArr[NumberOfOrders] = OrderType();
            LotSizeArr[NumberOfOrders] = OrderLots();
            RiskPercentArr[NumberOfOrders] = RiskPercent;
            OpenPriceArr[NumberOfOrders] = OrderOpenPrice();
            StopLossArr[NumberOfOrders] = OrderStopLoss();
            TakeProfitArr[NumberOfOrders] = OrderTakeProfit();
            NumberOfOrders++;
         
            //Send the new order
            LastProcessingTk = OrderTicket();
            
            //string message = CreateSignalMessage("OP",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            string message = CreateSignalMessage("OP",OrderTicket(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            //Print("Sending a signal...",message);
            
            InsertIntoPartialCheckArray(OrderTicket());
            
            SendOrderInformation(message);
            
            Print("[SendSignals]Sent: Open new order!!!");
         }//if ( OrderTicket()>LastProcessingTk)
      }//if(OrderSelect(orderArrays[index],SELECT_BY_TICKET))
   }//for(int index = 0; index < orderCount; index++)

   int temp_NumberOfOrders=0;
   bool updateFlag = false;
   //+------------------------------NEW ORDER PROCESSING----------------------------+
   
   //+----------------------------CURRENT ORDER PROCESSING--------------------------+
   for (int i = 0; i < NumberOfOrders; i++)
   {
      if (!OrderSelect(TicketArr[i],SELECT_BY_TICKET)) 
      {
         temp_TicketArr[temp_NumberOfOrders] = TicketArr[i];
         temp_TypeArr[temp_NumberOfOrders] = TypeArr[i];
         temp_LotSizeArr[temp_NumberOfOrders] = LotSizeArr[i];
         temp_RiskPercentArr[temp_NumberOfOrders] = RiskPercentArr[i];
         temp_OpenPriceArr[temp_NumberOfOrders] = OpenPriceArr[i];
         temp_StopLossArr[temp_NumberOfOrders] = StopLossArr[i];
         temp_TakeProfitArr[temp_NumberOfOrders] = TakeProfitArr[i];
      
         temp_NumberOfOrders++;    
      
         continue;
      }            
   
      if (OrderCloseTime() == 0)
      {
         if (OrderStopLoss()     != StopLossArr[i]    || 
             OrderTakeProfit()   != TakeProfitArr[i]  || 
             OrderLots()         != LotSizeArr[i]     || 
             OrderOpenPrice()    != OpenPriceArr[i]   || 
             RiskPercent         != RiskPercentArr[i])
         {
            //Order modified
            updateFlag = true;
            int modifyTicket = GetCloseTicket(OrderTicket());
            //message = CreateSignalMessage("MO",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            //message = CreateSignalMessage("MO",OrderTicket(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            message = CreateSignalMessage("MO",modifyTicket,OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
      
            //Print("Sending a signal...",message);
            
            SendOrderInformation(message);
            
            Print("[SendSignals]Sent: modify order!!!");
         }        
      
         temp_TicketArr[temp_NumberOfOrders] = TicketArr[i];
         temp_TypeArr[temp_NumberOfOrders] = TypeArr[i];
         temp_LotSizeArr[temp_NumberOfOrders] = OrderLots();
         temp_RiskPercentArr[temp_NumberOfOrders] = RiskPercent;
         temp_OpenPriceArr[temp_NumberOfOrders] = OrderOpenPrice();
         temp_StopLossArr[temp_NumberOfOrders] = OrderStopLoss();
         temp_TakeProfitArr[temp_NumberOfOrders] = OrderTakeProfit();
      
         temp_NumberOfOrders++;    
      }//if (OrderCloseTime() == 0)
      else        //Order already closed
      {
         //Order closed or cancelled
         updateFlag = true;
         if (OrderType() == OP_BUY || OrderType () == OP_SELL)
         {
            if(partialCloseOldTicket > 0 && partialCloseNewTicket > 0 && OrderTicket() == partialCloseOldTicket)
            {
               double masterClosePrice = OrderClosePrice();
               double masterLots = LotSizeArr[i];
            
               //Print("newPartialTk="+newPartialTk);
               if (OrderSelect(partialCloseNewTicket, SELECT_BY_TICKET))
               {
                  temp_TicketArr[temp_NumberOfOrders] = OrderTicket();
                  temp_TypeArr[temp_NumberOfOrders] = OrderType();
                  temp_LotSizeArr[temp_NumberOfOrders] = OrderLots();
                  temp_RiskPercentArr[temp_NumberOfOrders] = RiskPercent;
                  temp_OpenPriceArr[temp_NumberOfOrders] = OrderOpenPrice();
                  temp_StopLossArr[temp_NumberOfOrders] = OrderStopLoss();
                  temp_TakeProfitArr[temp_NumberOfOrders] = OrderTakeProfit();
                  temp_NumberOfOrders++;
               
                  double partialClosePercent = (masterLots - OrderLots()) / masterLots * 100;
               
                  //message = CreateSignalMessage("PL",OrderOpenTime(),OrderSymbol(), OrderType(),partialClosePercent,
                  //RiskPercent,masterClosePrice,OrderStopLoss(), OrderTakeProfit());                
                  //message = CreateSignalMessage("PL",OrderTicket(),OrderSymbol(), OrderType(),partialClosePercent,
                  //RiskPercent,masterClosePrice,OrderStopLoss(), OrderTakeProfit());
                  
                  int partialCloseTicket = GetPartialCloseTicket(partialCloseOldTicket, partialCloseNewTicket);
                  
                  message = CreateSignalMessage("PL",partialCloseTicket,OrderSymbol(), OrderType(),partialClosePercent,RiskPercent,masterClosePrice,OrderStopLoss(), OrderTakeProfit());
                  
                  SendOrderInformation(message);
                  
                  Print("[SendSignals]Sent: partially close order!!!");
               }
               else
               {
                  //temp_TicketArr[temp_NumberOfOrders] = TicketArr[i];
                  temp_TicketArr[temp_NumberOfOrders] = TicketArr[i];
                  temp_TypeArr[temp_NumberOfOrders] = TypeArr[i];
                  temp_LotSizeArr[temp_NumberOfOrders] = LotSizeArr[i];
                  temp_RiskPercentArr[temp_NumberOfOrders] = RiskPercentArr[i];
                  temp_OpenPriceArr[temp_NumberOfOrders] = OpenPriceArr[i];
                  temp_StopLossArr[temp_NumberOfOrders] = StopLossArr[i];
                  temp_TakeProfitArr[temp_NumberOfOrders] = TakeProfitArr[i];
      
                  temp_NumberOfOrders++;  
               }                  
            }
            else
            {               
               if (OrderLots() == LotSizeArr[i])     //fully close
               {
                  //message = CreateSignalMessage("CL",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderClosePrice(),OrderStopLoss(), OrderTakeProfit());                
                  //message = CreateSignalMessage("CL",OrderTicket(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderClosePrice(),OrderStopLoss(), OrderTakeProfit());
                  int closeTicket = GetCloseTicket(OrderTicket());
                  
                  message = CreateSignalMessage("CL",closeTicket,OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderClosePrice(),OrderStopLoss(), OrderTakeProfit());
                  
                  SendOrderInformation(message);
                   
                  Print("[SendSignals]Sent: close order!!!");
               }    
               else
               {
                  temp_TicketArr[temp_NumberOfOrders] = TicketArr[i];
                  temp_TypeArr[temp_NumberOfOrders] = TypeArr[i];
                  temp_LotSizeArr[temp_NumberOfOrders] = LotSizeArr[i];
                  temp_RiskPercentArr[temp_NumberOfOrders] = RiskPercentArr[i];
                  temp_OpenPriceArr[temp_NumberOfOrders] = OpenPriceArr[i];
                  temp_StopLossArr[temp_NumberOfOrders] = StopLossArr[i];
                  temp_TakeProfitArr[temp_NumberOfOrders] = TakeProfitArr[i];
                           
                  temp_NumberOfOrders++;
               }                 
            }
         }
         else
         {
            //message = CreateSignalMessage("DE",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            message = CreateSignalMessage("DE",OrderTicket(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            
            SendOrderInformation(message);
         
            Print("[SendSignals]Sent: delete order!!!");
         }
      }
   }
   //+----------------------------CURRENT ORDER PROCESSING--------------------------+

   //Retain the latest trades list
   if (updateFlag)
   {   
      for (i=0; i < temp_NumberOfOrders; i++)
      {
         TicketArr[i] = temp_TicketArr[i];
         TypeArr[i] = temp_TypeArr[i];
         LotSizeArr[i] = temp_LotSizeArr[i];
         RiskPercentArr[i] = temp_RiskPercentArr[i];
         OpenPriceArr[i] = temp_OpenPriceArr[i];
         StopLossArr[i] = temp_StopLossArr[i];
         TakeProfitArr[i] = temp_TakeProfitArr[i];
      }
      NumberOfOrders = temp_NumberOfOrders;
   }
}

void SendOrderInformation(string orderInformation)
{
   Print("[SendOrderInformation]Sending Order Information = " + orderInformation);
   SendToSocket(orderInformation);
}

int CheckPartialClose(string symbol, int ordertype, double lots, double openprice, datetime opentime, double stoploss,double takeprofit,int magicnumber)
{
   if (ordertype != OP_BUY && ordertype != OP_SELL)
   {
      return (-1);
   }
   
   int masterTicket = -1;
   //Print("---------------------");
   //Print("Check the order at openprice()="+openprice,",opentime()="+opentime,",lots="+lots);
   double point = MarketInfo(symbol, MODE_POINT);
   
   for (int retry=0; retry < 10; retry++)
   {
      bool success =true;
      for (int i=0; i<NumberOfOrders; i++)
      {
         //Print("TicketArr[i] = " + TicketArr[i]);
         if (!OrderSelect(TicketArr[i], SELECT_BY_TICKET)) 
         {
            success = false;
            continue;
         }            
         
         //Print("Ticket=" + OrderTicket() + ", OrderOpenTime()="+OrderOpenTime(),",OrderOpenPrice()="+OrderOpenPrice(), ", Lots=" + LotSizeArr[i]);         
         
         if (OrderType() == ordertype && MathAbs(OrderOpenPrice() - openprice) <= 1 * point && OrderOpenTime() == opentime)
         {
            //Print("LotSizeArr[" + i + "] = " + LotSizeArr[i] + " | lots = " + lots); 
            if (LotSizeArr[i]>lots)
            {
               masterTicket = OrderTicket();
               Print("[CheckPartialClose]Found masterTicket = " + masterTicket);
            }               
            else  masterTicket=0;
            
            success=true;        
            break;
         }      
      }
      
      if (success)
      {
         break;
      }
      else
      {
         Sleep(100);
      }
   }
   
   return (masterTicket);
}

//CreateSignalMessage("OP",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
string CreateSignalMessage(string cmd,int ticket,string sym, int type,double lots,double risk,double price,double sl, double tp)
{
   string message = "Trade:";
   
   double digits = MarketInfo(sym, MODE_DIGITS);
   
   if (SymbolPrefix != "")
   {      
      if (StringFind(sym, SymbolPrefix) == 0)
      {
         int len = StringLen(SymbolPrefix);
         sym = StringSubstr(sym, len);
      }
   }
   
   if (SymbolPostfix != "")
   {
      int len1 = StringLen(sym);      
      len = StringLen(SymbolPostfix);
      if (StringFind(sym, SymbolPostfix) == len1 - len)
      {
         sym = StringSubstr(sym, 0, len1 - len);  
      }   
   }
   
   if (cmd=="OP")
   {
      message = cmd + ticket + "_S_" + sym;
      message = message + "_T_" + type;
      message = message + "_L_" + DoubleToStr(lots,2);
      message = message + "_B_" + DoubleToStr(AccountBalance(),2);
      message = message + "_P_" + DoubleToStr(price,digits);
      message = message + "_sl_" + DoubleToStr(sl,digits);
      message = message + "_tp_" + DoubleToStr(tp,digits);      
   }
   else if (cmd=="MO")
   {
      message = cmd + ticket + "_S_" + sym;
      message = message + "_T_" + type;
      message = message + "_L_" + DoubleToStr(lots,2);
      message = message + "_B_" + DoubleToStr(AccountBalance(),2);
      message = message + "_P_" + DoubleToStr(price,digits);
      message = message + "_sl_" + DoubleToStr(sl,digits);
      message = message + "_tp_" + DoubleToStr(tp,digits); 
   }
   else if (cmd=="PL")
   {
      message = cmd + ticket + "_S_" + sym;
      message = message + "_T_" + type;
      message = message + "_CP_" + DoubleToStr(price,digits);
      message = message + "_PC_" + DoubleToStr(lots,2);
   }
   else if (cmd=="CL" || cmd=="DE")
   {
      message = cmd + ticket + "_S_" + sym;
      message = message + "_T_" + type;
      message = message + "_CP_" + DoubleToStr(price,digits);
   }
   return (message);
}
//+-------------------------------------------------------------------------------------+
//|                                   END Core Logic                                    |
//+-------------------------------------------------------------------------------------+

void InsertIntoPartialCheckArray(int ticket)
{
   string entry = "" + ticket;
   bool found = false;
   for(int i = 0; i < PartialCloseCount; i++)
   {
      string checkString = PartialCloseTickets[i];
      Print("[InsertIntoPartialCheckArray]Check String = " + checkString);
      
      int index = 0;
      string subString[20];
   
      int pos = 0;
      pos = StringFind(checkString , ",");
   
      subString[index] = StringSubstr(checkString, 0, pos);
      //Print(subString[index]);
      index++;
   
      checkString = StringSubstr(checkString, pos+1);
      //Print(message);
   
      while(pos != -1)
      {
         pos = StringFind(checkString , ",");
         subString[index] = StringSubstr(checkString, 0, pos);
         //Print(subString[index]);
         index++;
         checkString = StringSubstr(checkString, pos+1);
         //Print(orderInfo);
      }
      Print("[InsertIntoPartialCheckArray]Number of tickets found = " + index);
      
      for(int j = 0; j < index; j++)
      {
         if(entry == subString[j])
         {
            found = true;
            break;
         }
      }
   }
   if(!found)
   {
      PartialCloseTickets[PartialCloseCount] = entry;
      Print("[InsertIntoPartialCheckArray]Ticket#" + entry + " added to PartialCloseTickets");
      PartialCloseCount++;
   }
   else
   {
      Print("[InsertIntoPartialCheckArray]Ticket#" + entry + "already presenet in PartialCloseTickets");
   }
}

int GetPartialCloseTicket(int old, int new)
{
   string oldTkt = old + "";
   Print("[GetPartialCloseTicket]OLD = " + oldTkt + " | NEW = " + new);
   string result = -1;
   
   bool found = false;
   for(int i = 0; i < PartialCloseCount; i++)
   {
      string checkString = PartialCloseTickets[i];
      
      int index = 0;
      string subString[20];
   
      int pos = 0;
      pos = StringFind(checkString , ",");
   
      subString[index] = StringSubstr(checkString, 0, pos);
      //Print(subString[index]);
      index++;
   
      checkString = StringSubstr(checkString, pos+1);
      //Print(message);
   
      while(pos != -1)
      {
         pos = StringFind(checkString , ",");
         subString[index] = StringSubstr(checkString, 0, pos);
         //Print(subString[index]);
         index++;
         checkString = StringSubstr(checkString, pos+1);
         //Print(orderInfo);
      }
      Print("[GetPartialCloseTicket]Number of tickets found = " + index);
      for(int k = 0; k < index; k++)
      {
         Print("[GetPartialCloseTicket]Ticket found at " + k + " = " + subString[k]);
      }
      
      for(int j = 0; j < index; j++)
      {
         if(oldTkt == subString[j])
         {
            found = true;
            Print("[GetPartialCloseTicket]Old Ticket found at location " + j);
            result = subString[0];
            Print("[GetPartialCloseTicket]Result set to = " + result);
            break;
         }
      }
      
      if(found)
      {
         PartialCloseTickets[i] = PartialCloseTickets[i] + "," + new;
      }
      
      Print("New Ticket = " + new + " added | PartialCloseTickets[" + i + "] = " + PartialCloseTickets[i]);
   }
   return(StrToInteger(result));
}

int GetCloseTicket(int ticket)
{
   string tkt = ticket + "";
   Print("[GetCloseTicket]Ticket = " + tkt);
   string result = -1;
   
   bool found = false;
   for(int i = 0; i < PartialCloseCount; i++)
   {
      string checkString = PartialCloseTickets[i];
      
      int index = 0;
      string subString[10];
   
      int pos = 0;
      pos = StringFind(checkString , ",");
   
      subString[index] = StringSubstr(checkString, 0, pos);
      //Print(subString[index]);
      index++;
   
      checkString = StringSubstr(checkString, pos+1);
      //Print(message);
   
      while(pos != -1)
      {
         pos = StringFind(checkString , ",");
         subString[index] = StringSubstr(checkString, 0, pos);
         //Print(subString[index]);
         index++;
         checkString = StringSubstr(checkString, pos+1);
         //Print(orderInfo);
      }
      Print("[GetCloseTicket]Number of tickets found = " + index);
      
      for(int j = 0; j < index; j++)
      {
         if(tkt == subString[j])
         {
            found = true;
            Print("[GetCloseTicket]Old Ticket found at location " + j);
            result = subString[0];
            Print("[GetCloseTicket]Result set to = " + result);
            break;
         }
      }
   }
   return(StrToInteger(result));
}