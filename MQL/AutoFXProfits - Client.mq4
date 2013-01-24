//+------------------------------------------------------------------+
//|                                       AutoFXProfits - Client.mq4 |
//|                                  Copyright 2013, Auto FX Profits |
//+------------------------------------------------------------------+
#property copyright "Copyright 2013, Auto FX Profits"

#import "kernel32.dll"
void GetLocalTime(int& TimeArray[]);
void GetSystemTime(int& TimeArray[]);
int  GetTimeZoneInformation(int& TZInfoArray[]);
#import

#import "Communication Library.dll"
   bool     SpawnClientTerminal(int handle, string terminalName);
   bool     WriteToFile(string message, string fileName, string terminalName);
   string   ReadFromFile(string fileName, string terminalName);
   bool     FileDeleteExternal(string fileName, string terminalName);
#import

#include <stdlib.mqh>

extern bool UseMM = true;
extern double RiskPercent = 2.0;
extern double FixedLot = 0.1;
extern double DefaultSL = 20;

//=============GLOBAL VARIABLES=============
string PredefinedPrefix = "";
string PredefinedPostfix = "";

string SymbolPrefix  = "";                      //The symbol prefix for the system
string SymbolPostfix = "";                      //The symbol postfix for the system

static int      Token;

int PointFactor = 1;
double  GMT_Shift = 0;
datetime LastProcessingTk;
string LastReceivingTime;
string EA_StartTime;
int LastReceivingId = 0;
double Slippage = 5;
bool SuffixesRead = false;
int WaitingTime = 0;

string Signals[100];
int SignalIdx;

string Order_comment = "AutoFXProfits";

//+------------------------------------------------------------------+
//| expert initialization function                                   |
//+------------------------------------------------------------------+
int init()
{
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
   
   WriteAccountInfoToFile();
   
   string mode = "";
   if(IsTesting())
   {
      mode = "TEST";
   }
   else
   {
      mode = "LIVE";
   }
   
   SpawnClientTerminal(WindowHandle(Symbol(), Period()), "AutoFXProfitsClientTerminal");
   
   int TZInfoArray[43];         
   int dst = GetTimeZoneInformation(TZInfoArray);
   GMT_Shift = TZInfoArray[0];
   
   if(dst == 2)
   {
      GMT_Shift += TZInfoArray[42];
   }
   
   datetime gmtTime = ConvertGMTTime(TimeLocal(),GMT_Shift);
   
   LastProcessingTk = 0; 
   
   Print("GMT Time = " + gmtTime);
   EA_StartTime = ConvertToString(gmtTime);
   LastReceivingTime = EA_StartTime;
   Print("EA Start Time / Last Receiving Time = " + LastReceivingTime);
   
   //SetSymbolPrefixAndPostfix();
   
   if (!AuthenticateReceiver())
   {
      Comment("TradeMirror Receiver: NO PERMISION TO RECEIVE SIGNALS!!!");
      Print("TradeMirror Receiver: NO PERMISION TO RECEIVE SIGNALS!!!");
      Token=0;
   }
   else
   {
      Comment("TradeMirror Receiver is READY!");
      Print("TradeMirror Receiver is READY!");
      Token=1;
   }
   
   string Sym = Symbol();
   double SymPoints = MarketInfo( Sym, MODE_POINT  );
   double SymDigits = MarketInfo( Sym, MODE_DIGITS );
  
   if( SymPoints == 0.001   ) { SymPoints = 0.01;   SymDigits = 3; }
   else if( SymPoints == 0.00001 ) { SymPoints = 0.0001; SymDigits = 5; }
   
   if (SymDigits==3 || SymDigits==5)
   {
      PointFactor=10;
   }
   
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
      while(!IsStopped() && IsExpertEnabled() && IsConnected())
      {
         if(!SuffixesRead)
         {
            if (TimeLocal() >= WaitingTime)
            {
               ReadSuffixes();
               WaitingTime = TimeLocal() + 1;
            }
         }
         ReceiveSignals();
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

bool AuthenticateReceiver()
{
   //Authenticate the receiver here
   return(true);
}

datetime ConvertGMTTime(datetime sourcetime, double gmt_shift)
{
   datetime gmt = sourcetime + GMT_Shift * 60;
   return (gmt); 
}

string ConvertToString(datetime dt)
{
    string str1 = TimeToStr(dt,TIME_DATE);
    string str2 = TimeToStr(dt,TIME_SECONDS);
    
    string str3 = str1 + str2;
    
    //2011.01.13
    str1 = StringSubstr(str1,0,4)+StringSubstr(str1,5,2) + StringSubstr(str1,8,2);
    
    //10:02:03
    str2 = StringSubstr(str2,0,2)+StringSubstr(str2,3,2) + StringSubstr(str2,6,2);
    
    return (str1+str2);
}

void Idle()
{
    if (IsTradeContextBusy())
    {
        Sleep(100);
    }
    else
    {
        Sleep(1000);
    }
}
//+-------------------------------------------------------------------------------------+
//|                                    END Utility                                      |
//+-------------------------------------------------------------------------------------+

//+-------------------------------------------------------------------------------------+
//|                                  START Core Logic                                   |
//+-------------------------------------------------------------------------------------+

bool ReceiveSignals()
{
   string orderInformation;
   
   orderInformation = ReadOrderInformation();
   //Print("Return size = " + StringLen(orderInformation));
   
   if(StringLen(orderInformation) > 0)
   {
      Print("[ReceiveSignals]Order Information Received = " + orderInformation);
      
      //FileDelete("orders.csv");
      //bool     FileDeleteExternal(string fileName, string terminalName)
      if(FileDeleteExternal("orders.csv", "AutoFXProfitsClientTerminal"))
      {
         Print("[ReceiveSignals] Order File Deleted");
      }
      else
      {
         Print("[ReceiveSignals] Order file not deleted");
      }
      
      int index2 = StringFind(orderInformation, ";", 0);
      Print("Index2 = " + index2);
      if (index2 != -1)
      {
         string tempTime = LastReceivingTime;
         Print("Temp Time = " + tempTime);
                           
         int index1 = 0;
         
         //222,manual,OP9476112_S_USDJPY_T_0_L_1.00_P_83.03_sl_0.00_tp_0.00,20110113093101;[EOF]
   
         while (index2 != -1)
         {
            string signalStr = StringSubstr(orderInformation, index1, index2 - index1);
      
            signalStr = StringTrimLeft(signalStr);
      
            string items[];
      
            split(items, signalStr, ",");
      
            if (items[3] >= tempTime)
            {
               tempTime = items[3];                
               
               int type = GetSignalType(items[2]);
               Print("Signal Type = " + type);
   
               Signals[SignalIdx]= items[1]+ "," + items[2] + "," + items[3];
               Print("Signals[" + SignalIdx + "] = " + Signals[SignalIdx]);

               SignalIdx++;
            }
                     
            index1 = index2+1;
            index2 = StringFind(orderInformation, ";", index1); 
            
            int lastId = StrToInteger(items[0]);
            if (lastId > LastReceivingId)
            {
               LastReceivingId = lastId;
            }
         }
         
         if(LastReceivingTime < tempTime)
         {
            LastReceivingTime = tempTime;
         }                   
      }
      
      //updare the LastReceivingTime 
   
      //Print(LastReceivingId);
    
      if (SignalIdx>0)
      {
         string tempSignals[1000];
         int tempSignalIdx = 0;
         for (int i=0; i<SignalIdx; i++)
         {
            //Print("Incoming trade - at timestring " + tempTime);
            if (PlaceTrade(Signals[i]))
            {
               
            }
            else
            {
               tempSignals[tempSignalIdx]=Signals[i];
               tempSignalIdx++;
            }
         }
         if (tempSignalIdx<SignalIdx)
         {
            for (i=0;i<tempSignalIdx; i++)
            {
               Signals[i]=tempSignals[i];
            }
            SignalIdx=tempSignalIdx;
         }
      }
   }
}

//Reads order information from file
string ReadOrderInformation()
{
   string order = ReadFromFile("orders.csv", "AutoFXProfitsClientTerminal");
   return(order);
}

int GetSignalType(string signal)
{
   string cmd = StringSubstr(signal,0,2);
        
   if (cmd == "OP")
   {
      int ind1 = StringFind(signal,"_S_");
      int ticket = StrToInteger(StringSubstr(signal,2, ind1-2));

      int ind2 = StringFind(signal,"_T_");
      string symbol = StringSubstr(signal,ind1+3,ind2-ind1-3);
                  
      ind1=ind2;
      ind2 = StringFind(signal,"_L_");
      int type = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
         
      return (type);      
   }   
   return (-1);         
}

string GetCorrectSymbol(string prefix, string core, string postfix)
{
   string symbol = prefix + core + postfix;
      
   if (MarketInfo(symbol,MODE_POINT)==0)
   {
      bool flag = false;
      if (StringFind(core,"GOLD")!=-1)
      {
         //try XAUUSD
         symbol = prefix + "XAUUSD" + postfix;
         if (MarketInfo(symbol,MODE_POINT)>0)
         {
            flag = true;
         }
      }
      else if (StringFind(core,"XAUUSD")!=-1)
      {
         //try GOLD
         symbol = prefix + "GOLD" + postfix;
         if (MarketInfo(symbol,MODE_POINT)>0)
         {
            flag = true;
         }
      }
      else if (StringFind(core,"SILVER")!=-1)
      {
         symbol = prefix + "XAGUSD" + postfix;
         if (MarketInfo(symbol,MODE_POINT)>0)
         {
            flag = true;
         }
      }
      else if (StringFind(core,"XAGUSD")!=-1)
      {
         symbol = prefix + "SILVER" + postfix;
         if (MarketInfo(symbol,MODE_POINT)>0)
         {
            flag = true;
         }
      }
      
      if (!flag)
      {
         symbol = "";
      }
   }
   
   return (symbol);
}

int GetOrderTicket(int magic)
{
   Print("Magic Number = " + magic);
   int total = OrdersTotal();
   int ticket=-1;

   for(int pos=total-1;pos>=0;pos--)
   {    
      if(OrderSelect(pos,SELECT_BY_POS)==false) continue;

      if ( OrderMagicNumber() == magic )
      {
         ticket = OrderTicket();
         break;                  
      }          
   }
   
   if (ticket>0) return (ticket);
   
   
   total = OrdersHistoryTotal();
   ticket=-1;
   for(pos=total-1;pos>=0;pos--)
   {    
      if(OrderSelect(pos,SELECT_BY_POS,MODE_HISTORY)==false) continue;

      if ( OrderMagicNumber() == magic )
      {
         ticket = OrderTicket();
         break;                  
      }          
   }
         
   return (ticket);      
}

double CalculateLotsize(string sym, double riskPercent, double stoploss)
{
   InitSymbol(sym);
   if (stoploss==0) stoploss = DefaultSL * PointFactor;
   
   double bal = AccountFreeMargin();
                   
   double dollarsToRisk = riskPercent * bal/100;
   double dollarsPerTick = MarketInfo(sym, MODE_TICKVALUE);
   double newLotSize = dollarsToRisk/(stoploss*dollarsPerTick);
                
   newLotSize = NormalizeLotSize(sym,newLotSize);   
   
   Print("Balance = " + bal + " | dollarsToRisk = " + dollarsToRisk + " | dollarsPerTick = " + 
         dollarsPerTick + " | newLotSize = " + newLotSize + " | newLotSize = " + newLotSize);
          
   return (newLotSize);
}

double NormalizeLotSize(string sym, double lots)
{
   int digit_lot = 1;
   
   double permittedMaxLot = AccountFreeMargin()/MarketInfo(sym,MODE_MARGINREQUIRED);
   double minLot = MarketInfo(sym,MODE_MINLOT);
   
   Print("Max Lot = " + permittedMaxLot + " | Min Lot = "  + minLot);
   
   if(lots<minLot)
   {
      lots = minLot;
      Print("Lots set to minLot");
   }
   if(lots>permittedMaxLot)
   {
      lots = permittedMaxLot;
      Print("Lots set to max lot");
   }
   
   if(MarketInfo(sym,MODE_LOTSTEP)>=0.01 && MarketInfo(sym,MODE_LOTSTEP)<0.1) digit_lot=2;   
   if(MarketInfo(sym,MODE_LOTSTEP)>=0.1 && MarketInfo(sym,MODE_LOTSTEP)<1) digit_lot=1;
   if(MarketInfo(sym,MODE_LOTSTEP)>=1) digit_lot=0;
   
   lots=NormalizeDouble(lots,digit_lot);
   Print("Lots = " + lots + " with normalization to " + digit_lot + " digits");
   
   return (lots);
}

void InitSymbol(string symbol)
{
   double points = MarketInfo( symbol, MODE_POINT  );
   double digits = MarketInfo( symbol, MODE_DIGITS );
   //---
  
   if( points == 0.001   ) { points = 0.01;   digits = 3; }
   else if( points == 0.00001 ) { points = 0.0001; digits = 5; }
         
   if (StringFind(Symbol(),"XAUUSD")!=-1||StringFind(Symbol(),"GOLD")!=-1)
   {
      if (digits==2)
      {
         PointFactor = 10;
      }
   }
   else if (StringFind(Symbol(),"XAGUSD")!=-1||StringFind(Symbol(),"SILVER")!=-1)
   {
      if (digits==3)
      {
         PointFactor = 10;
      }
   }
   else
   {   
      if (digits==3 || digits==5)
      {
         PointFactor = 10;
      }
   }           
}


bool PlaceTrade(string message, bool pendingOrderOnly=false)
{   
   bool ret = true;
   string items[];
   split(items,message,",");
   
   string signal = items[1];
   
   string cmd = StringSubstr(signal,0,2);
   
   //Print("signal="+signal);
   
   if (cmd == "OP")
   {
      Print("Received: open order");
      
      int ind1 = StringFind(signal, "_S_");
      int ticket = StrToInteger(StringSubstr(signal, 2, ind1 - 2));
      Print("Ticket = ", ticket);

      int ind2 = StringFind(signal, "_T_");
      string coreSymbol = StringSubstr(signal, ind1 + 3, ind2 -ind1 - 3);
      
      string symbol = GetCorrectSymbol(SymbolPrefix,coreSymbol,SymbolPostfix);
            
      if (symbol == "")
      {
         Print("invalid symbol : " + symbol);
         return (true);  
      }      
      
      Print("Symbol = ", symbol);
            
      int tk = GetOrderTicket(ticket);
      
      if (tk == -1 && OrderSelect(ticket, SELECT_BY_TICKET))
      {
         if (OrderSymbol() == symbol)
         {
            Print("Order already existing");
            tk = ticket;       
         }
      }
      
      Print("tk = " + tk);
      
      if (tk == -1)
      {                                               
         ind1=ind2;
         ind2 = StringFind(signal,"_L_");
         int type = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
         Print(" Type = ", type);
            
         ind1=ind2;
         ind2 = StringFind(signal,"_B_");
         double lots = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
         Print("Lots = " ,lots);
         
         ind1=ind2;
         ind2 = StringFind(signal,"_P_");
         //double risk = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));         
         double balance = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));         
         
         //Print("risk=",risk);
         
         //lots = CalculateMM (symbol,RiskPercent);                           
         
         ind1=ind2;
         ind2 = StringFind(signal,"_sl_");
         double price = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
         Print("Price = ", price);
            
         ind1=ind2;
         ind2 = StringFind(signal,"_tp_");
         double sl = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
         Print("SL = ", sl);
            
         ind1=ind2;
         ind2 = StringLen(signal);
         double tp = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
         Print("TP = ", tp);
         
         double digits = MarketInfo(symbol,MODE_DIGITS);
         double point = MarketInfo(symbol,MODE_POINT);  
         
         {
             
            {
               bool success = true;       
               tk=-1;        
               for (int i=0; i<10; i++)
               {
                  RefreshRates();
                  double ask = MarketInfo(symbol,MODE_ASK);
                  double bid = MarketInfo(symbol,MODE_BID);                  
                     
                  if (type == OP_BUY)
                  {  
                     if (UseMM && RiskPercent>0)     
                     {
                        if (sl > 0)                        
                           lots = CalculateLotsize(symbol,RiskPercent,(ask-sl)/point); 
                        else
                           lots = CalculateLotsize(symbol,RiskPercent,DefaultSL*PointFactor);                            
                     }                        
                     else lots = FixedLot;  
                          
                     {
                        Print("Sending order with the following parameters: Symbol = " + symbol +
                                                                        " | Type = " + OP_BUY +
                                                                        " | Lots = " + lots + 
                                                                        " | Price = " + MarketInfo(symbol, MODE_ASK) + 
                                                                        " | Slippage = " + (Slippage * PointFactor) +
                                                                        " | Comment = " + Order_comment +
                                                                        " | Magic Number = " + ticket);
                        tk = OrderSend(symbol, OP_BUY, lots, MarketInfo(symbol, MODE_ASK), Slippage * PointFactor, 0, 0, Order_comment, ticket);
                        if(sl == 0)
                        {
                           sl =  MarketInfo(symbol, MODE_ASK) - (DefaultSL * GetPipSizePrecision(symbol));
                           Print("Default SL = " + sl);
                        }
                     }
                  }
                  else if (type == OP_SELL)
                  {
                     if (UseMM && RiskPercent>0)     
                     {
                        if (sl>0)                        
                           lots = CalculateLotsize(symbol,RiskPercent,(sl-bid)/point); 
                        else
                           lots = CalculateLotsize(symbol,RiskPercent,DefaultSL*PointFactor);                            
                     }                        
                     else lots = FixedLot; 
                                       
                     {
                        tk = OrderSend(symbol, OP_SELL, lots, MarketInfo(symbol,MODE_BID), Slippage*PointFactor, 0, 0, Order_comment, ticket); 
                        if(sl == 0)
                        {
                           sl = MarketInfo(symbol,MODE_BID) + (DefaultSL * GetPipSizePrecision(symbol));
                           Print("Default SL = " + sl);
                        }  
                     }
                  }
                  else
                  {             
                     if (UseMM && RiskPercent>0)     
                     {
                        if (sl>0)                        
                           lots = CalculateLotsize(symbol,RiskPercent,MathAbs(price-sl)/point); 
                        else
                           lots = CalculateLotsize(symbol,RiskPercent,DefaultSL*PointFactor);                            
                     }                        
                     else lots = FixedLot;
                             
                     {                             
                        tk = OrderSend(symbol, type, lots, NormalizeDouble(price,digits), Slippage*PointFactor, 0, 0, Order_comment, ticket);   
                     }                        
                  }
                              
                  if (tk >= 0)
                  {            
                     break;
                  }
                         
                  Idle();                           
               }
                 
               if(tk<0)
               {
                  success = false;  
                  Print("OrderSend failed with error #",GetLastError());                
               }
               else if (tk>0 && (sl>0 || tp>0))
               {
                  success=false;
                  for (i=0; i<10; i++)
                  {               
                     RefreshRates();
               
                     if (OrderSelect(tk,SELECT_BY_TICKET))
                     {
                        Print("About to modify. Ticket = " + OrderTicket() + " | SL = " + (NormalizeDouble(sl, digits)) + " | TP = " + (NormalizeDouble(tp,digits)));
                        if (OrderModify(OrderTicket(),OrderOpenPrice(),NormalizeDouble(sl, digits),NormalizeDouble(tp,digits),OrderExpiration()))
                        {
                           success=true;
                           break;
                        }
                        else
                        {
                           Print("Order not modified. Error = " + ErrorDescription(GetLastError()));
                        }
                     }
                     Idle();
                  }
               }
               else
               {
                  success=true;
               }
            
               if (!success)
               {
                  ret = false;              
               }
            }            
         }                  
      }
      else
      {
         success=true;
         if (OrderSelect(tk,SELECT_BY_TICKET))
         {
            if (OrderCloseTime()!=0) return (true);
            
            if (OrderStopLoss()==0 || OrderTakeProfit()==0)
            {
               //get sl/tp
               ind1=ind2;
               ind2 = StringFind(signal,"_L_");
               type = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
               //Print("type=",type);
            
               ind1=ind2;
               ind2 = StringFind(signal,"_B_");
               lots = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
               //Print("lots=",lots);
               
               ind1=ind2;
               ind2 = StringFind(signal,"_P_");
               //risk = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));               
               balance = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));               
               //Print("risk=",risk);   
                                    
         
               ind1=ind2;
               ind2 = StringFind(signal,"_sl_");
               price = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
               //Print("price=",price);
            
               ind1=ind2;
               ind2 = StringFind(signal,"_tp_");
               sl = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
               //Print("sl=",sl);
            
               ind1=ind2;
               ind2 = StringLen(signal);
               tp = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
               //Print("tp=",tp);
         
               digits = MarketInfo(OrderSymbol(),MODE_DIGITS);
               
               sl = NormalizeDouble(sl,digits);
               tp = NormalizeDouble(tp,digits);
               
               if ( (sl!=OrderStopLoss()) || (tp!=OrderTakeProfit()) )
               {
                  success=false;
               
                  for (i=0; i<2; i++)
                  {               
                     RefreshRates();
               
                     if (OrderModify(OrderTicket(),OrderOpenPrice(),NormalizeDouble(sl, digits),NormalizeDouble(tp,digits),OrderExpiration()))
                     {
                        success=true;
                        break;
                     }
                     Idle();
                  }
               }
               else
               {
                  success=true;
               }
            }
         }
         
         if (!success)
         {
            ret = false;
         }
      }               
   }        
   else if (cmd == "MO")
   {
      Print("Received: modify order");

      ind1 = StringFind(signal,"_S_");
      ticket = StrToInteger(StringSubstr(signal,2, ind1-2));
      //Print("ticket=",ticket);


      ind2 = StringFind(signal,"_T_");
      coreSymbol = StringSubstr(signal,ind1+3,ind2-ind1-3);
      
      symbol = GetCorrectSymbol(SymbolPrefix,coreSymbol,SymbolPostfix);
            
      if (symbol=="")
      {
         Print("invalid symbol : " + symbol);
         return (true);  
      } 
            
      //Print("symbol=",symbol);

      ind1=ind2;
      ind2 = StringFind(signal,"_L_");
      type = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      //Print("type=",type);
   
      ind1=ind2;
      ind2 = StringFind(signal,"_B_");
      lots = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      //Print("lots=",lots);

      ind1=ind2;
      ind2 = StringFind(signal,"_P_");
      //risk = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      balance = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      //Print("risk=",risk);

      ind1=ind2;
      ind2 = StringFind(signal,"_sl_");
      price = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      //Print("price=",price);

      ind1=ind2;
      ind2 = StringFind(signal,"_tp_");
      sl = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
      //Print("sl=",sl);

      ind1=ind2;
      ind2 = StringLen(signal);
      tp = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
      //Print("tp=",tp);
      
      digits = MarketInfo(symbol,MODE_DIGITS);
      
      {
         tk = GetOrderTicket(ticket);
         if (tk>=0)
         {
            success = false;
            for (i=0; i<10; i++)
            { 
               if (OrderSelect(tk, SELECT_BY_TICKET))
               {
                  if (OrderCloseTime()!=0) return (true);
                  
                  if (OrderType()==OP_BUY || OrderType()==OP_SELL)
                  {
                     sl = NormalizeDouble(sl,digits);
                     tp = NormalizeDouble(tp,digits);
                     if (sl!=OrderStopLoss() || tp!=OrderTakeProfit())
                     {
                        if (OrderModify(OrderTicket(),OrderOpenPrice(),NormalizeDouble(sl,digits),NormalizeDouble(tp,digits),OrderExpiration()))
                        {
                           success=true;
                           break;
                        }
                     }
                     else
                     {
                        success=true;
                        break;
                     }
                  }
                  else//pending order
                  {
                     sl = NormalizeDouble(sl,digits);
                     tp = NormalizeDouble(tp,digits);
                     price = NormalizeDouble(price,digits);
                     
                     if (sl!=OrderStopLoss() || tp!=OrderTakeProfit() || price!=OrderOpenPrice())
                     {
                        if (OrderModify(OrderTicket(),NormalizeDouble(price,digits),NormalizeDouble(sl,digits),NormalizeDouble(tp,digits),OrderExpiration()))
                        {
                           success=true;
                           break;
                        }
                     }
                     else
                     {
                        success=true;
                        break;
                     }
                  }               
               }   
               Idle();   
            }   
            if (!success)
            {
               //ret = false;            
            }         
            
                        
         }
      }
   }
   else if (cmd == "PL")
   {
      Print("Received: partially close order");
      //PL9476112_S_USDJPY_T_0_CP_82.99_PC_10.20_TK_8298292
      
      ind1 = StringFind(signal,"_S_");
      ticket = StrToInteger(StringSubstr(signal,2, ind1-2));
      Print("Ticket = ", ticket);

      ind2 = StringFind(signal,"_T_");
      coreSymbol = StringSubstr(signal,ind1+3,ind2-ind1-3);
      
      symbol = GetCorrectSymbol(SymbolPrefix,coreSymbol,SymbolPostfix);
      //Print("Symbol = " + symbol);
            
      if (symbol == "")
      {
         Print("invalid symbol : " + symbol);
         return (true);  
      } 
      
      Print("Symbol = " + symbol);

      ind1=ind2;
      ind2 = StringFind(signal,"_CP_");
      type = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      Print("Type = ", type);
      
      
      ind1=ind2;
      ind2 = StringFind(signal,"_PC_");
      price = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
      Print("Price = " + price);
      
      ind1=ind2;
      ind2 = StringLen(signal);
      double percentClose = StrToDouble( StringSubstr(signal,ind1+4,ind2-ind1-4));
      
      Print("Prcent Close = "+ percentClose);
      {
         tk = GetOrderTicket(ticket);
         Print("tk = " + tk);
         if (tk>0 && OrderSelect(tk, SELECT_BY_TICKET))
         {    
            if (OrderCloseTime()!=0) return (true);
            type = OrderType();
            success = false;
            lots = NormalizeLotSize(OrderSymbol(),percentClose*OrderLots()/100);
            Print("Lots to close = " + lots);
            
            if (type==OP_BUY)
            {
               for(i=0; i<20; i++)
               {
                  RefreshRates();
                  if (OrderClose(OrderTicket(), lots,MarketInfo(OrderSymbol(),MODE_BID),Slippage*PointFactor)) 
                  {
                     success = true;
                     break;
                  }
                  Idle();
               }
            } 
            else if (type==OP_SELL)
            {
               for (i=0; i<10; i++)
               { 
                  RefreshRates();
                  if(OrderClose(OrderTicket(), lots,MarketInfo(OrderSymbol(),MODE_ASK),Slippage*PointFactor )) 
                  {
                     success = true;
                     break;
                  }
                  Idle();
               }
            }
            else
            {
               for (i=0; i<10; i++)
               {
                  if (OrderDelete(OrderTicket())) 
                  {
                     success = true;
                     break;
                  }
               
                  Idle();
               }
            }
            
            if (!success)
            {
               //ret = false;           
            }
                        
         }     
      }          
   }
   else if (cmd == "CL" || cmd =="DE")
   {
      Print("Received: close order");
      //CL9476112_S_USDJPY_T_0_CP_82.99
      
      ind1 = StringFind(signal,"_S_");
      ticket = StrToInteger(StringSubstr(signal,2, ind1-2));
      //Print("ticket=",ticket);

      ind2 = StringFind(signal,"_T_");
      coreSymbol = StringSubstr(signal,ind1+3,ind2-ind1-3);
      
      symbol = GetCorrectSymbol(SymbolPrefix,coreSymbol,SymbolPostfix);
            
      if (symbol=="")
      {
         Print("invalid symbol : " + symbol);
         return (true);  
      } 
      
      //Print("symbol=",symbol);

      ind1=ind2;
      ind2 = StringFind(signal,"_CP_");
      type = StrToDouble( StringSubstr(signal,ind1+3,ind2-ind1-3));
      //Print("type=",type);
      
      {
         tk = GetOrderTicket(ticket);
         if (tk>0 && OrderSelect(tk, SELECT_BY_TICKET))
         {    
            if (OrderCloseTime()!=0) return (true);
            
            type = OrderType();
            success = false;
            if (type==OP_BUY)
            {
               for(i=0; i<20; i++)
               {
                  RefreshRates();
                  if (OrderClose(OrderTicket(), OrderLots(),MarketInfo(OrderSymbol(),MODE_BID),Slippage*PointFactor)) 
                  {
                     success = true;
                     break;
                  }
                  Idle();
               }
            } 
            else if (type==OP_SELL)
            {
               for (i=0; i<10; i++)
               { 
                  RefreshRates();
                  if(OrderClose(OrderTicket(), OrderLots(),MarketInfo(OrderSymbol(),MODE_ASK),Slippage*PointFactor )) 
                  {
                     success = true;
                     break;
                  }
                  Idle();
               }
            }
            else
            {
               for (i=0; i<10; i++)
               {
                  if (OrderDelete(OrderTicket())) 
                  {
                     success = true;
                     break;
                  }
               
                  Idle();
               }
            }
            
            if (!success)
            {
               //ret = false;           
            }
                        
         }     
      }                      
   }
   return (ret);
}

//+-------------------------------------------------------------------------------------+
//|                                   END Core Logic                                    |
//+-------------------------------------------------------------------------------------+
void WriteAccountInfoToFile()
{
   WriteToFile(StringConcatenate("accountNumber: ", AccountNumber()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("currency: \"", AccountCurrency()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("isDemo: ", IsDemo()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("accountServer: \"", AccountServer(), "\""), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("balance: ", AccountBalance()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("equity: ", AccountEquity()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("floatingPL: ", AccountProfit()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("credit: ", AccountCredit()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("marginInUse: ", AccountMargin()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("freeMargin: ", AccountFreeMargin()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("openOrders: ", OrdersTotal()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("closedOrders: ", OrdersHistoryTotal()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
   WriteToFile(StringConcatenate("brokerTime: ", TimeCurrent()), "accountinfo.txt", "AutoFXProfitsClientTerminal");
}

//Return the Pip Size in double
double GetPipSizePrecision(string symbol)
{
   int digits = MarketInfo(symbol, MODE_DIGITS);

   if (digits == 5 || digits == 4)
      return(0.0001);
   else if (digits == 3 || digits == 2)
      return(0.01);
}

//Reads order information form file
void ReadSuffixes()
{
   string order = ReadFromFile("suffixes.csv", "AutoFXProfitsClientTerminal");
      
   if(order != "NULL")
   {
      PredefinedPrefix = order;
      PredefinedPostfix = order;
      Print("[ReadSuffixes] Read Suffixes = " + order);
      SetSymbolPrefixAndPostfix();
      SuffixesRead = true;
      
      if(FileDeleteExternal("suffixes.csv", "AutoFXProfitsClientTerminal"))
      {
         Print("[ReadSuffixes] suffixes.csv deleted");
      }
      else
      {
         Print("[ReadSuffixes] suffixes.csv could not be deleted");
      }
   }
   else
   {
      Print("[ReadSuffixes] No Suffixes or Error");
   }
}
