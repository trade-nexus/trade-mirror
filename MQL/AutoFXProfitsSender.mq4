//+------------------------------------------------------------------+
//|                                          AutoFXProfitsSender.mq4 |
//|                      Copyright 2012, Aurora Solutions (pvt) Ltd. |
//|                                   http://www.aurorasolutions.org |
//+------------------------------------------------------------------+
#property copyright "Copyright 2012, Aurora Solutions (pvt) Ltd."
#property link      "http://www.aurorasolutions.org"

#import "Trade Mirror Communication Library.dll"
   bool SendToSocket(string orderInfo);
#import

#include <stdlib.mqh>

//=============GLOBAL VARIABLES=============
string PredefinedPrefix = ".,m,fx,_fx,$";       //Defined set of Symbol Prefixes
string PredefinedPostfix = ".,m,fx,_fx,$";      //Defined set of Symbol Postfixes

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
      Print("Please Allow DLL Import!");
      return (0);
   }
   
   SetSymbolPrefixAndPostfix();
   
   if (!AuthenticateSender())
   {
      Alert("TradeMirror Sender: NO PERMISION TO POST SIGNALS!!!");
      Print("TradeMirror Sender: NO PERMISION TO POST SIGNALS!!!");
      Token=0;
   }
   else
   {
      Alert("TradeMirror Sender is READY!");
      Print("TradeMirror Sender is READY!");
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
   if (MarketInfo("EURUSD", MODE_POINT) != 0)      //If there is not prefix & postfix in our symbol
   {
      SymbolPrefix = "";
      SymbolPostfix = "";
      
   }
   else
   {
      string preList[];
      string posList[];
      
      split(preList, PredefinedPrefix, ',');                     
      split(posList, PredefinedPostfix, ',');
      
      bool found = false;
      for (int i=0; i<ArraySize(posList); i++)
      {
         string testSymbol = "EURUSD" + posList[i];
         if (MarketInfo( testSymbol, MODE_POINT  )!=0)
         {
            SymbolPrefix = "";
            SymbolPostfix = posList[i];       
            found = true;
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
                  found = true;
                  break;
               }
            }
         }   
      }         
   }
}

void split(string& arr[], string str, string sym) 
{
   ArrayResize(arr, 0);
   string item;
   int pos, size;
  
   int len = StringLen(str);
   for (int i=0; i < len;) 
   {
     pos = StringFind(str, sym, i);
     if (pos == -1) pos = len;
    
     if (pos-i == 0)
     {
       item = "";
     }
     else
     {
       item = StringSubstr(str, i, pos-i);
       item = StringTrimLeft(item);
       item = StringTrimRight(item);
     }
    
     size = ArraySize(arr);
     ArrayResize(arr, size+1);
     arr[size] = item;
    
     i = pos+StringLen(sym);
   }
  
   size = ArraySize(arr);
   if (size == 1)
   {
     ArrayResize(arr, size+1);
     arr[size] = "";
   }
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
         if (OrderTicket() > LastProcessingTk)                
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
               //Print("partial close order #"+partialCloseOldTicket + ", new ticket #" + partialCloseNewTicket);
               break;                       
            }
            //Print("new ordertime = ",TimeToStr(OrderOpenTime(),TIME_DATE|TIME_SECONDS));
         
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
            
            SendOrderInformation(message);
            
            Print("Sent: Open new order!!!");
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
            //message = CreateSignalMessage("MO",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
            message = CreateSignalMessage("MO",OrderTicket(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderOpenPrice(),OrderStopLoss(), OrderTakeProfit());
      
            //Print("Sending a signal...",message);
            
            SendOrderInformation(message);
            
            Print("Sent: modify order!!!");
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
               
                  //message = CreateSignalMessage("PL",OrderOpenTime(),OrderSymbol(), OrderType(),partialClosePercent,RiskPercent,masterClosePrice,OrderStopLoss(), OrderTakeProfit());                
                  message = CreateSignalMessage("PL",OrderTicket(),OrderSymbol(), OrderType(),partialClosePercent,RiskPercent,masterClosePrice,OrderStopLoss(), OrderTakeProfit());
                  
                  SendOrderInformation(message);
                  
                  Print("Sent: partially close order!!!");
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
            else
            {               
               if (OrderLots() == LotSizeArr[i])     //fully close
               {
                  //message = CreateSignalMessage("CL",OrderOpenTime(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderClosePrice(),OrderStopLoss(), OrderTakeProfit());                
                  message = CreateSignalMessage("CL",OrderTicket(),OrderSymbol(), OrderType(),OrderLots(),RiskPercent,OrderClosePrice(),OrderStopLoss(), OrderTakeProfit());
                  
                  SendOrderInformation(message);
                   
                  Print("Sent: close order!!!");
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
         
            Print("Sent: delete order!!!");
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
         if (!OrderSelect(TicketArr[i], SELECT_BY_TICKET)) 
         {
            success = false;
            continue;
         }            
         
         //Print("Ticket=" + OrderTicket() + ", OrderOpenTime()="+OrderOpenTime(),",OrderOpenPrice()="+OrderOpenPrice(), ", Lots=" + LotSizeArr[i]);         
         
         if (OrderType() == ordertype && MathAbs(OrderOpenPrice() - openprice) <= 1 * point && OrderOpenTime() == opentime)
         {
            //Print("get here"); 
            if (LotSizeArr[i]>lots)
            {
               masterTicket = OrderTicket();
               //Print("Found masterTicket="+masterTicket);
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