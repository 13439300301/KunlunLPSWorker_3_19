using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    public class RedisLuaScript
    {
        #region 基础定义
        //一些账户类型定义，储值，积分，成长值
        public const string ACCOUNT_TYPE_STORED_VALUE = "StoredValueAccounts";
        public const string ACCOUNT_TYPE_POINTS = "PointsAccounts";
        public const string ACCOUNT_TYPE_GROWTH = "GrowthAccounts";

        public const string STORED_VALUE_ACCOUNT_TYPE = "StoredValueAccountTypes"; //账户类型（只存本金和透支）

        public const string OVERDRAFT_DEFINE_AMOUNT = "OverdraftDefineAmount"; // 透支定义额度

        // 预授权后缀
        public const string AUTHORIZATION = "Auth";
        public const string AUTHORIZATION_TOTAL = "AuthTotal";

        // 卡号段
        public const string MEMBERSHIP_CARD_NUMBER_SEGMENT_KEY = @"LPS:NumberSegmentId";
        #endregion 基础定义

        #region 账户相关
        /// <summary>
        /// 获取某卡的某个账户的值，如果值为空，则代表账户不存在
        /// </summary>
        public const string GET_ACCOUNT_VALUE = @"
local returnValue = nil
local K_CardId = nil
local F_MembershipCardAccountId = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    F_MembershipCardAccountId=KEYS[2]
end

-- 获取账户id
local V_AccountValue = redis.call('hget', K_CardId, F_MembershipCardAccountId)
if(V_AccountValue)
then
    --V_AccountValue 非空
    returnValue = V_AccountValue
end

return { returnValue }
";

        /// <summary>
        /// 更新某卡某账户的余额
        /// </summary>
        public const string UPDATE_ACCOUNT_BALANCE = @"
local returnValue = nil
local K_CardId = nil
local F_MembershipCardAccountId = nil
local V_AccountValue = nil
local K_CardOverdraft = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    F_MembershipCardAccountId=KEYS[2]
end
if(KEYS[3])
then
    K_CardOverdraft=KEYS[3]
end
if(ARGV[1])
then
    V_AccountValue=ARGV[1]
end

returnValue = V_AccountValue

-- 判断账户是否已经存在
local accountValue = redis.call('hget', K_CardId, F_MembershipCardAccountId)
if (accountValue)
then
    --如果已经存在，则取redis里的账户余额
    returnValue = accountValue  
else
    redis.call('hset', K_CardId, F_MembershipCardAccountId, V_AccountValue)
    
    local overdraftAccountId = redis.call('hget',K_CardOverdraft,'OverdraftAccount')
    if(overdraftAccountId ~= F_MembershipCardAccountId)
    then
        --更新卡的总余额，不包括透支账户
        local totalAmount = redis.call('hget', K_CardId, 'Total')
        if (totalAmount)
        then
            redis.call('hset', K_CardId, 'Total', string.format('%.8f',(totalAmount + V_AccountValue)))
        else
            redis.call('hset', K_CardId, 'Total', V_AccountValue)
        end
    end
end

return { returnValue }
";

        /// <summary>
        /// 更新某卡某账户的储值账户类型
        /// </summary>
        public const string UPDATE_STORED_VALUE_ACCOUNT_TYPE = @"
local returnValue = nil
local K_CardId = nil
local F_StoredValueAccountType = nil
local V_MembershipCardAccountId = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    F_StoredValueAccountType=KEYS[2]
end
if(ARGV[1])
then
    V_MembershipCardAccountId=ARGV[1]
end

returnValue = V_MembershipCardAccountId

-- 判断账户是否已经存在
local storedValueAccountType = redis.call('hget', K_CardId, F_StoredValueAccountType)
if (storedValueAccountType)
then
    --如果已经存在，则取值
    returnValue = storedValueAccountType  
else
    redis.call('hset', K_CardId, F_StoredValueAccountType, V_MembershipCardAccountId)
end

return { returnValue }
";

        /// <summary>
        /// 更新某卡某透支账户的定义额度
        /// </summary>
        public const string UPDATE_OVERDRAFT_DEFINE_AMOUNT = @"
local returnValue = nil
local K_CardId = nil
local F_MembershipCardAccountId = nil
local V_AccountValue = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    F_MembershipCardAccountId=KEYS[2]
end
if(ARGV[1])
then
    V_AccountValue=ARGV[1]
end

returnValue = V_AccountValue

-- 判断账户是否已经存在
local overdraftDefineAmount = redis.call('hget', K_CardId, F_MembershipCardAccountId)
if (overdraftDefineAmount)
then
    --如果已经存在，则取值
    returnValue = overdraftDefineAmount  
else
    redis.call('hset', K_CardId, F_MembershipCardAccountId, V_AccountValue)
end

return { returnValue }
";

        /// <summary>
        /// 获取透支账户的id，余额，定义额度
        /// </summary>
        public const string GET_CARD_ACCOUNT_OVERDRAFT = @"
local K_CardId = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end

--透支账户余额-key
local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'

--透支账户id-key
local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'

--透支账户定义额度-key
local keyOverdraftDefineAmount = 'LPS:Card:' ..K_CardId.. ':OverdraftDefineAmount'

local overdraftAccountId = redis.call('hget',keyCardStoredType,'OverdraftAccount') --获取透支账户id
local overdraftBalance = 0       --透支余额
local overdraftDefineAmount = 0  --透支金额上限

if(overdraftAccountId)
then
    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalanceTemp)
    then
        overdraftBalance = overdraftBalanceTemp
    end

    --获取透支账户额度
    local overdraftDefineAmountTemp = redis.call('hget', keyOverdraftDefineAmount, overdraftAccountId)
    if(overdraftDefineAmountTemp)
    then
        overdraftDefineAmount = overdraftDefineAmountTemp
    end
end

return {overdraftAccountId,overdraftBalance,overdraftDefineAmount}

";

        /// <summary>
        /// 调整会员透支账户的定义额度，同时更新透支账户余额
        /// </summary>
        public const string ADJUST_OVERDRAFT_ACCOUNT_DEFINE_AMOUNT = @"
local K_CardId = nil
local V_NewDefineAmount = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(ARGV[1])
then
    V_NewDefineAmount = ARGV[1]
end

--透支账户余额-key
local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'

--透支账户id-key
local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'

--透支账户定义额度-key
local keyOverdraftDefineAmount = 'LPS:Card:' ..K_CardId.. ':OverdraftDefineAmount'

local overdraftAccountId = redis.call('hget',keyCardStoredType,'OverdraftAccount') --获取透支账户id
local overdraftBalance = 0       --透支余额
local overdraftDefineAmount = 0  --透支金额上限
local transactionAmount --透支账户增加的金额

if(overdraftAccountId)
then
    --获取透支账户额度
    local overdraftDefineAmountTemp = redis.call('hget', keyOverdraftDefineAmount, overdraftAccountId)
    if(overdraftDefineAmountTemp)
    then
        overdraftDefineAmount = overdraftDefineAmountTemp
    end

    --更新透支账户额度
    redis.call('hset', keyOverdraftDefineAmount, overdraftAccountId, V_NewDefineAmount)

    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalanceTemp)
    then
        overdraftBalance = overdraftBalanceTemp
    end
    
    local overdraftBalance = string.format('%.8f',(V_NewDefineAmount - overdraftDefineAmount + overdraftBalance))
    transactionAmount = string.format('%.8f',(V_NewDefineAmount - overdraftDefineAmount))
    redis.call('hset', keyCardIdStoredValueAccounts, overdraftAccountId, overdraftBalance)
end

return {overdraftAccountId,overdraftBalance,V_NewDefineAmount, transactionAmount}
";

        /// <summary>
        /// 积分支付计算，并更新账户余额
        /// </summary>
        public const string CALCULATE_UPDATE_POINT_ACCOUNT = @"
local V_TransactionPoint = nil --传入的积分
local K_CardId = nil
local V_MembershipCardAccountId = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(ARGV[1])
then
    V_MembershipCardAccountId = ARGV[1]
end
if(ARGV[2])
then
    V_TransactionPoint = ARGV[2]
end

local accountValue = redis.call('hget', K_CardId, V_MembershipCardAccountId)  --积分账户余额

if(accountValue)
then
    --判断余额是否足够支付
    if(accountValue + V_TransactionPoint < 0)
    then
        return { 'ERROR', 'INSUFFICIENT_BALANCE' }
    else              
        --计算账户余额
        local balance = string.format('%.8f',(accountValue + V_TransactionPoint))
        --更新账户余额
        redis.call('hset', K_CardId, V_MembershipCardAccountId, balance)

        --更新卡的总积分
        local totalPoint = redis.call('hget', K_CardId, 'Total')
        if(totalPoint)
        then
            redis.call('hset', K_CardId, 'Total', string.format('%.8f',(totalPoint + V_TransactionPoint)))
        end
        return { 'OK', balance }
    end
else
    return { 'ERROR', 'Not Exist Point Account' }
end

";

        /// <summary>
        /// 成长值支付计算，并更新账户余额
        /// </summary>
        public const string CALCULATE_UPDATE_GROWTH_ACCOUNT = @"
local V_TransactionGrowth = nil --传入的成长值
local K_CardId = nil
local V_MembershipCardAccountId = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(ARGV[1])
then
    V_MembershipCardAccountId = ARGV[1]
end
if(ARGV[2])
then
    V_TransactionGrowth = ARGV[2]
end

local accountValue = redis.call('hget', K_CardId, V_MembershipCardAccountId)  --成长值账户余额

if(accountValue)
then
    --判断余额是否足够支付
    if(accountValue + V_TransactionGrowth < 0)
    then
        return { 'ERROR', 'INSUFFICIENT_BALANCE' }
    else              
        --计算账户余额
        local balance = string.format('%.8f',(accountValue + V_TransactionGrowth))
        --更新账户余额
        redis.call('hset', K_CardId, V_MembershipCardAccountId, balance)

        --更新卡的总成长值
        local totalPoint = redis.call('hget', K_CardId, 'Total')
        if(totalPoint)
        then
            redis.call('hset', K_CardId, 'Total', string.format('%.8f',(totalPoint + V_TransactionGrowth)))
        end
        return { 'OK', balance }
    end
else
    return { 'ERROR', 'Not Exist Point Account' }
end

";

        /// <summary>
        /// 变更储值账户余额，增减皆可（会员卡不存在透支账户的情况下）
        /// 传入的json格式：[{"membershipCardAccountId":"117","transactionAmount":"90"},{"membershipCardAccountId":"118","transactionAmount":"-90"}]
        /// ------------------------------------------
        /// 返回的json格式：[{"membershipCardAccountId":"117","transactionAmount":"90","balance":"199","lastBalance":"109"},{"membershipCardAccountId":"118","transactionAmount":"-90","balance":"19","lastBalance":"109"}]
        /// </summary>
        public const string CHANGE_STOREDVALUE_ACCOUNT_BALANCE = @"
local jsonStr = nil
local K_CardId = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(ARGV[1])
then
    jsonStr = ARGV[1]
end

local accountInfos = cjson.decode(jsonStr)

local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'
local overdraftAccountId = redis.call('hget',keyCardStoredType,'OverdraftAccount') --获取透支账户id

--如果存在透支账户
if(overdraftAccountId)
then
    return { 'Err', 'Please Use Other Lua Script!'}
end

local cardTransactionAmount = 0 --卡增加的总金额

local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'
--循环更新每个账户的余额
for index,accountInfo in ipairs(accountInfos) do
    local thisBalance = 0
    
    local accountBalance = redis.call('hget',keyCardIdStoredValueAccounts,accountInfo['membershipCardAccountId'])
    if(accountBalance)
    then
        thisBalance = string.format('%.8f',(accountBalance + accountInfo['transactionAmount']))

        cardTransactionAmount = string.format('%.8f',(cardTransactionAmount + accountInfo['transactionAmount']))

        --更新账户余额
        redis.call('hset', keyCardIdStoredValueAccounts, accountInfo['membershipCardAccountId'], thisBalance)

        --要返回的数组
        accountInfo['balance'] = thisBalance
        accountInfo['lastBalance'] = accountBalance
    end
end

local cardThisBalance = 0 --卡总余额
--更新卡的总余额，可支付账户的总余额，不包括透支账户
local cardLastBalance = redis.call('hget', keyCardIdStoredValueAccounts, 'Total')
if(cardLastBalance)
then
    cardThisBalance = string.format('%.8f',(cardLastBalance + cardTransactionAmount))
    redis.call('hset', keyCardIdStoredValueAccounts, 'Total', cardThisBalance)
end

local returnJson = cjson.encode(accountInfos)
return { 'OK', returnJson,cardThisBalance,cardLastBalance }
";

        /// <summary>
        /// 增加储值账户余额，按账户（会员卡存在透支账户的情况下）
        /// 传入的json格式：[{"membershipCardAccountId":"117","transactionAmount":"90"}]
        /// ------------------------------------------
        /// 返回的json格式：[{"membershipCardAccountId":"117","transactionAmount":"90","balance":"199","lastBalance":"109"}]
        /// </summary>
        public const string ADD_STOREDVALUE_ACCOUNT_BALANCE_EXIST_OVERDRAFT = @"
local jsonStr = nil
local K_CardId = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(ARGV[1])
then
    jsonStr = ARGV[1]
end

local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'

local accountInfos = cjson.decode(jsonStr)

local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'
local overdraftAccountId = redis.call('hget',keyCardStoredType,'OverdraftAccount') --获取透支账户id
local principalAccountId = redis.call('hget',keyCardStoredType,'PrincipalAccount') --获取本金账户id

local overdraftBalance = 0
local overdraftDefineAmount = 0  --透支金额上限
--如果存在透支账户
if(overdraftAccountId)
then
    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalanceTemp)
    then
        overdraftBalance = overdraftBalanceTemp
    end
    --获取透支账户定义额度
    local keyOverdraftDefineAmount = 'LPS:Card:' ..K_CardId.. ':OverdraftDefineAmount'
    local overdraftDefineAmountTemp = redis.call('hget', keyOverdraftDefineAmount, overdraftAccountId)
    if(overdraftDefineAmountTemp)
    then
        overdraftDefineAmount = overdraftDefineAmountTemp
    end
else
    return { 'Err', 'Please Use Other Lua Script!'}
end

local principalBalance = 0 --本金上次余额
--如果存在本金账户
if(overdraftAccountId)
then
    --获取本金账户余额
    local principalBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, principalAccountId)
    if(principalBalanceTemp)
    then
        principalBalance = principalBalanceTemp
    end
end

local principalAmount = 0  --本金要增加的钱
local overdraftAmount = 0 --透支要增加的钱

--根据json中的信息，获取本金、透支账户要增加的金额
for index,accountInfo in ipairs(accountInfos) do
    if(accountInfo['membershipCardAccountId'] == principalAccountId)
    then
        principalAmount = string.format('%.8f',(principalAmount + accountInfo['transactionAmount']))
    end
    if(accountInfo['membershipCardAccountId'] == overdraftAccountId)
    then
        overdraftAmount = string.format('%.8f',(overdraftAmount + accountInfo['transactionAmount']))
    end
end

--如果存在透支账户
if(overdraftAccountId)
then
    --若是增加金额后，透支账户溢出（超出透支上限），则溢出的补充到本金账户；若增加金额后，透支账户不满额，则本金的补充到透支账户
    --根据上述逻辑，重新计算本金和透支账户要增加的金额
    if(overdraftBalance + overdraftAmount - overdraftDefineAmount > 0)
    then
        principalAmount = string.format('%.8f',(principalAmount + overdraftAmount + overdraftBalance - overdraftDefineAmount))
        overdraftAmount = string.format('%.8f',(overdraftDefineAmount - overdraftBalance))
    else
        --如果本金补足透支后还有剩余
        if (principalAmount + overdraftAmount + overdraftBalance - overdraftDefineAmount >= 0)
        then
            principalAmount = string.format('%.8f',(principalAmount - overdraftDefineAmount + overdraftAmount + overdraftBalance))
            overdraftAmount = string.format('%.8f',(overdraftDefineAmount - overdraftBalance))
        else
            principalAmount = 0
            overdraftAmount =string.format('%.8f',(overdraftAmount + principalAmount))
        end
    end
end

local cardTransactionAmount = 0 --卡增加的总金额
local returnArr = {}
local i = 1

--如果本金要增加金额，则更新本金账户
if((principalAmount-0)>0)
then

    redis.call('hset', keyCardIdStoredValueAccounts, principalAccountId, string.format('%.8f',(principalBalance + principalAmount)))

    cardTransactionAmount = cardTransactionAmount + principalAmount    

    --要返回的数组
    local arr = {}
    arr['membershipCardAccountId'] = principalAccountId
    arr['transactionAmount'] = principalAmount
    arr['balance'] = string.format('%.8f',(principalBalance + principalAmount))
    arr['lastBalance'] = principalBalance

    returnArr[i] = arr
    i = i + 1
end

--如果透支要增加金额，则更新透支账户
if((overdraftAmount-0)>0)
then
    redis.call('hset', keyCardIdStoredValueAccounts, overdraftAccountId, string.format('%.8f',(overdraftBalance + overdraftAmount)))

    --要返回的数组
    local arr = {}
    arr['membershipCardAccountId'] = overdraftAccountId
    arr['transactionAmount'] = overdraftAmount
    arr['balance'] = string.format('%.8f',(overdraftBalance + overdraftAmount))
    arr['lastBalance'] = overdraftBalance

    returnArr[i] = arr
    i = i + 1
end

local cardThisBalance = 0 --卡总余额
--更新卡的总余额，可支付账户的总余额，不包括透支账户
local cardLastBalance = redis.call('hget', keyCardIdStoredValueAccounts, 'Total')
if(cardLastBalance)
then
    cardThisBalance = string.format('%.8f',(cardLastBalance + cardTransactionAmount))
    redis.call('hset', keyCardIdStoredValueAccounts, 'Total', cardThisBalance)
end

local returnJson = cjson.encode(returnArr)
return { 'OK', returnJson,cardThisBalance,cardLastBalance }
";

        /// <summary>
        /// 扣减储值账户余额，按账户（会员卡存在透支账户的情况下）
        /// 传入的json格式：[{"membershipCardAccountId":"117","transactionAmount":"-90"}]
        /// ------------------------------------------
        /// 返回的json格式：[{"membershipCardAccountId":"117","transactionAmount":"-90","balance":"109","lastBalance":"199"}]
        /// </summary>
        public const string DEDUCT_ACCOUNT_BALANCE = @"
local jsonStr = nil
local K_CardId = nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(ARGV[1])
then
    jsonStr = ARGV[1]
end

local accountInfos = cjson.decode(jsonStr)

local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'
local overdraftAccountId = redis.call('hget',keyCardStoredType,'OverdraftAccount')
local principalAccountId = redis.call('hget',keyCardStoredType,'PrincipalAccount')

local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'
local cardTotalBalance = 0
--获取卡总余额
local cardTotalBalance = redis.call('hget', keyCardIdStoredValueAccounts, 'Total')

local principalBalance = 0 --本金上次余额
local overdraftBalance = 0 --透支上次余额

--如果存在透支账户
if(overdraftAccountId)
then
    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalanceTemp)
    then
        overdraftBalance = overdraftBalanceTemp
    end
    --获取本金账户余额
    local principalBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, principalAccountId)
    if(principalBalanceTemp)
    then
        principalBalance = principalBalanceTemp
    end
else
    return { 'Err', 'Please Use Other Lua Script!'}
end

--预授权总金额
local keyAuthTotal = 'LPS:Card:' ..K_CardId.. ':AuthTotal'
local totalAuthAmount = redis.call('GET', keyAuthTotal) 
if(totalAuthAmount)
then
    totalAuthAmount = totalAuthAmount
else
    totalAuthAmount = 0
end

local principalAmount = 0  --本金要扣除的钱
local overdraftAmount = 0 --透支要扣除的钱

for index,accountInfo in ipairs(accountInfos) do
    if(accountInfo['membershipCardAccountId'] == principalAccountId)
    then
        principalAmount = string.format('%.8f',(principalAmount + accountInfo['transactionAmount']))
    end
    if(accountInfo['membershipCardAccountId'] == overdraftAccountId)
    then
        overdraftAmount = string.format('%.8f',(overdraftAmount + accountInfo['transactionAmount']))
    end
end

--判断余额是否足够支付
if(principalBalance + overdraftBalance + principalAmount + overdraftAmount < 0)
then
    return { 'ERROR', 'INSUFFICIENT_BALANCE' }
end

local returnArr = {}
local i = 1

local cardTransactionAmount = 0 --卡扣减的总金额

--重新计算本金和透支账户要扣减的金额，如果本金不足，扣透支的钱
if(principalBalance + principalAmount < 0)
then
    overdraftAmount = string.format('%.8f',(overdraftAmount + principalAmount + principalBalance))    
    principalAmount = 0 - principalBalance
end
cardTransactionAmount = cardTransactionAmount + principalAmount

if(principalAmount - 0 < 0)
then
    --更新账户余额
    redis.call('hset', keyCardIdStoredValueAccounts, principalAccountId, string.format('%.8f',(principalBalance + principalAmount)))
    --要返回的数组
    local arr = {}
    arr['membershipCardAccountId'] = principalAccountId
    arr['transactionAmount'] = principalAmount
    arr['balance'] = string.format('%.8f',(principalBalance + principalAmount))
    arr['lastBalance'] = principalBalance

    returnArr[i] = arr
    i = i + 1
end
if(overdraftAmount - 0 < 0)
then
    redis.call('hset', keyCardIdStoredValueAccounts, overdraftAccountId, string.format('%.8f',(overdraftBalance + overdraftAmount)))
    --要返回的数组
    local arr = {}
    arr['membershipCardAccountId'] = overdraftAccountId
    arr['transactionAmount'] = overdraftAmount
    arr['balance'] = string.format('%.8f',(overdraftBalance + overdraftAmount))
    arr['lastBalance'] = overdraftBalance

    returnArr[i] = arr
    i = i + 1
end

local cardThisBalance = 0 --卡总余额
--更新卡的总余额，可支付账户的总余额，不包括透支账户
local cardLastBalance = redis.call('hget', keyCardIdStoredValueAccounts, 'Total')
if(cardLastBalance)
then
    cardThisBalance = string.format('%.8f',(cardLastBalance + cardTransactionAmount))
    redis.call('hset', keyCardIdStoredValueAccounts, 'Total', cardThisBalance)
end

local returnJson = cjson.encode(returnArr)
return { 'OK', returnJson,cardThisBalance,cardLastBalance }
";

        /// <summary>
        /// 扣减卡值，按卡（一般都是支付动作）
        /// 传入的json格式：{"paymentWay":"1","membershipCardAccountIds":["119"]}
        ///  如果是按照优先级支付（paymentWay:1），则数组是按照优先级排列的，第一个优先级最高
        /// ------------------------------------------
        /// 返回的json格式：[{"membershipCardAccountId":"119","transactionAmount":"-279","balance":"0","lastBalance":"279"},{"membershipCardAccountId":"120","transactionAmount":"-10","balance":"90","lastBalance":"100"}]
        /// </summary>
        public const string DEDUCT_CARD_FEE = @"
local jsonStr = nil
local V_TransactionAmount = nil --传入的金额为负值
local K_CardId = nil
local K_CardIdAuth = nil  
local F_CancelAuthNumber = nil --要释放的预授权号
local K_AuthTotal =  nil

if(KEYS[1])
then
    K_CardId = KEYS[1]
end
if(KEYS[2])
then
    F_CancelAuthNumber = KEYS[2]
end
if(ARGV[1])
then
    jsonStr = ARGV[1]
end
if(ARGV[2])
then
    V_TransactionAmount = ARGV[2]
end

local tbPaymentWay = cjson.decode(jsonStr)

local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'
local totalAmount = 0  --可支付账户总金额
--获取所有可支付账户的总金额,index 表示索引, accountId 表示单个账户
for index,membershipCardAccountId in ipairs(tbPaymentWay['membershipCardAccountIds']) do
    local accountValue = redis.call('hget', keyCardIdStoredValueAccounts, membershipCardAccountId)
    if(accountValue)
    then
        totalAmount = string.format('%.8f',(totalAmount + accountValue))
    end
end

--预授权总金额
local keyAuthTotal = 'LPS:Card:' ..K_CardId.. ':AuthTotal'
local totalAuthAmount = redis.call('GET', keyAuthTotal) 
if(totalAuthAmount)
then
    totalAuthAmount = totalAuthAmount
else
    totalAuthAmount = 0
end

local keyCancelAuth = 'LPS:Card:' ..K_CardId.. ':Auth'
--获取要释放的预授权的金额
local cancelAuthAmount = redis.call('HGET',keyCancelAuth,F_CancelAuthNumber)
if(cancelAuthAmount)
then
    --如果有值，直接删除此预授权，代表释放
    redis.call('HDEL',keyCancelAuth,F_CancelAuthNumber)
    redis.call('INCRBYFLOAT', keyAuthTotal, -cancelAuthAmount)
else
    cancelAuthAmount = 0
end

local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'
local overdraftAccountId = redis.call('hget',keyCardStoredType,'OverdraftAccount')
local overdraftBalance = 0
--如果存在透支账户
if(overdraftAccountId)
then
    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('hget', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalance)
    then
        overdraftBalance = overdraftBalanceTemp
    end
end

local cardLastBalance = redis.call('hget', keyCardIdStoredValueAccounts, 'Total') --卡上次总余额
local cardThisBalance = cardLastBalance --卡当前总余额

--判断余额是否足够支付
if(totalAmount + V_TransactionAmount - totalAuthAmount + cancelAuthAmount + overdraftBalance < 0)
then
    return { 'ERROR', 'INSUFFICIENT_BALANCE' }
else
    --计算透支账户需要支付的金额（负数）
    local overdraftAmount = nil
    if(totalAmount + V_TransactionAmount - totalAuthAmount + cancelAuthAmount < 0)
    then
        overdraftAmount = string.format('%.8f',(totalAmount + V_TransactionAmount - totalAuthAmount + cancelAuthAmount))
    else 
        overdraftAmount = 0
    end

    local returnArr = {} --要返回的数组

    local i = 1 --索引
    --如果透支账户也需要支付，说明其他账户余额肯定都会为0
    if(overdraftAmount - 0 < 0)
    then
        for index,membershipCardAccountId in ipairs(tbPaymentWay['membershipCardAccountIds']) do
            local accountValue = redis.call('hget', keyCardIdStoredValueAccounts, membershipCardAccountId)
            
            --如果账户余额为0的话，那么此账户不进行任何操作
            if(accountValue - 0 ~= 0)
            then
                --更新账户余额
                redis.call('hset', keyCardIdStoredValueAccounts, membershipCardAccountId, 0)

                --卡当前余额
                if(cardThisBalance)
                then
                    cardThisBalance = string.format('%.8f',(cardThisBalance - accountValue))
                end

                --要返回的数组
                local arr = {}
                arr['membershipCardAccountId'] = membershipCardAccountId
                arr['transactionAmount'] = string.format('%.8f',(0 - accountValue))
                arr['balance'] = '0'
                arr['lastBalance'] = accountValue
                returnArr[i] = arr
                i = i + 1
            end
        end

        --更新透支账户余额
        redis.call('hset', keyCardIdStoredValueAccounts, overdraftAccountId, string.format('%.8f',(overdraftBalance + overdraftAmount)))
        
        --要返回的数组
        local arr1 = {}
        arr1['membershipCardAccountId'] = overdraftAccountId
        arr1['transactionAmount'] = overdraftAmount
        arr1['balance'] = string.format('%.8f',(overdraftBalance + overdraftAmount))
        arr1['lastBalance'] = overdraftBalance
        returnArr[i] = arr1
    else
        --0是按比例支付
        if(tbPaymentWay['paymentWay']=='0')
        then
            for index,membershipCardAccountId in ipairs(tbPaymentWay['membershipCardAccountIds']) do
                local accountValue = redis.call('hget', keyCardIdStoredValueAccounts, membershipCardAccountId)
                --如果账户余额为0的话，那么此账户不进行任何操作
                if(accountValue - 0 ~= 0)
                then
                    --计算当前账户需要支付的金额
                    local currentTransactionAmount = string.format('%.8f',(V_TransactionAmount * accountValue / totalAmount)) 
                    --计算账户余额
                    local balance = string.format('%.8f',(accountValue + currentTransactionAmount))
                    --更新账户余额
                    redis.call('hset', keyCardIdStoredValueAccounts, membershipCardAccountId, balance)
                    
                    --卡当前余额
                    if(cardThisBalance)
                    then
                        cardThisBalance = string.format('%.8f',(cardThisBalance + currentTransactionAmount))
                    end

                    --要返回的数组
                    local arr = {}
                    arr['membershipCardAccountId'] = membershipCardAccountId
                    arr['transactionAmount'] = currentTransactionAmount
                    arr['balance'] = balance
                    arr['lastBalance'] = accountValue
                    returnArr[i] = arr
                    i = i + 1
                end
            end
        else
            local nextTransactionAmount = V_TransactionAmount --下一个账户可能要交易的金额
            for index,membershipCardAccountId in ipairs(tbPaymentWay['membershipCardAccountIds']) do
                local accountValue = redis.call('hget', keyCardIdStoredValueAccounts, membershipCardAccountId)
                --如果账户余额为0的话，那么此账户不进行任何操作
                if(accountValue - 0 ~= 0 and nextTransactionAmount - 0 ~= 0)
                then
                    local currentTransactionAmount = 0    --当前账户需要支付的金额
                    local balance = 0  -- 当前账户的余额
                    --判断当前账户余额是否足够支付
                    if(accountValue + nextTransactionAmount < 0)
                    then
                        --计算当前账户需要支付的金额
                        currentTransactionAmount = string.format('%.8f',(0 - accountValue))
                        --计算账户余额
                        balance = 0
                        --更新账户余额
                        redis.call('hset', keyCardIdStoredValueAccounts, membershipCardAccountId, balance)
                        
                        --卡当前余额
                        if(cardThisBalance)
                        then
                            cardThisBalance = string.format('%.8f',(cardThisBalance + currentTransactionAmount))
                        end

                        nextTransactionAmount = string.format('%.8f',(nextTransactionAmount - currentTransactionAmount))
                    else
                        --计算当前账户需要支付的金额
                        currentTransactionAmount = nextTransactionAmount
                        
                        --计算账户余额
                        balance = string.format('%.8f',(accountValue + currentTransactionAmount))
                        --更新账户余额
                        redis.call('hset', keyCardIdStoredValueAccounts, membershipCardAccountId, balance)
                        
                        --卡当前余额
                        if(cardThisBalance)
                        then
                            cardThisBalance = string.format('%.8f',(cardThisBalance + currentTransactionAmount))
                        end

                        nextTransactionAmount = string.format('%.8f',(nextTransactionAmount - currentTransactionAmount))
                    end

                    --要返回的数组
                    local arr = {}
                    arr['membershipCardAccountId'] = membershipCardAccountId
                    arr['transactionAmount'] = currentTransactionAmount
                    arr['balance'] = balance
                    arr['lastBalance'] = accountValue
                    returnArr[i] = arr
                    i = i + 1
                end
            end
        end
    end
    
    --更新卡总余额
    redis.call('hset', keyCardIdStoredValueAccounts, 'Total', cardThisBalance)
    
    local returnJson = cjson.encode(returnArr)

    return { 'OK', returnJson, cardThisBalance, cardLastBalance }
end

";


        /// <summary>
        /// 积分池计算，只扣减
        /// </summary>
        public const string CALCULATE_UPDATE_POINTPOOL_ACCOUNT = @"

local JsonStr = nil --传入的会员卡JOSN
local Value = 0

if(KEYS[1])
then
    JsonStr = KEYS[1]
end
if(ARGV[1])
then
    Value = tonumber(ARGV[1]+0)
end

local JsonList = cjson.decode(JsonStr)
local sum=0

--循环更新每个账户的余额
for index,item in ipairs(JsonList) do
    local v=redis.call('hget', item['CardId'], item['MembershipCardAccountId'])
    if(v)
    then
        sum = string.format('%.8f',(sum + v))
        item['LastBalance'] = v
    end
end

if(sum + Value >= 0)
then
    --循环更新每个账户的余额
    for index,item in ipairs(JsonList) do
        local v=redis.call('hget', item['CardId'], item['MembershipCardAccountId'])
        if(tonumber(v) > 0)
        then
            if(tonumber(Value+0)<0)
            then 
                local tempValue = Value
                Value = string.format('%.8f',(v + Value))
                if (tonumber(Value+0) < 0)
                then
                    --更新账户余额
                    redis.call('hset', item['CardId'], item['MembershipCardAccountId'], 0)
                    item['Balance'] = 0
                    item['Points'] = 0 - tonumber(v)

                    --更新卡的总积分
                    local totalPoint = redis.call('hget', item['CardId'], 'Total')
                    if(totalPoint)
                    then
                        redis.call('hset', item['CardId'], 'Total', 0)
                    end
                else
                    --更新账户余额
                    redis.call('hset', item['CardId'], item['MembershipCardAccountId'],Value)
                    item['Balance'] = Value
                    item['Points'] = tempValue

                    --更新卡的总积分
                    local totalPoint = redis.call('hget', item['CardId'], 'Total')
                    if(totalPoint)
                    then
                        redis.call('hset', item['CardId'], 'Total', Value)
                    end
                end
            else
                Value = string.format('%.8f',(v + Value))
            end
        end
    end
else
    return { 'INSUFFICIENT_BALANCE', 'Point Insufficient' }
end

return { 'OK', cjson.encode(JsonList), Value }
";

        #endregion 账户相关

        #region 生成账户

        /// <summary>
        /// 创建账户(本金，透支)
        /// </summary>
        public const string CREATE_ACCOUNT_STOREDVALUEACCOUNTTYPES = @"
local returnValue = nil

--获取卡值的卡id
local K_CardId = nil

--获取卡值的账户类型id
local K_MembershipCardAccountId = nil

--获取账户类型的id(本金,透支)
local K_StoredValueAccountTypesId = nil

--获取透支定义额度的id
local K_OverdraftDefineAmountId = nil

--余额
local V_AccountValue = nil

--额度
local V_CreditLine = nil

--获取卡值的账户类型(本金,透支)
local V_AccountType = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    K_MembershipCardAccountId=KEYS[2]
end
if(KEYS[3])
then
    K_StoredValueAccountTypesId=KEYS[3]
end
if(KEYS[4])
then
    K_OverdraftDefineAmountId=KEYS[4]
end
if(ARGV[1])
then
    V_AccountValue=ARGV[1]
end
if(ARGV[2])
then
    V_CreditLine=ARGV[2]
end
if(ARGV[3])
then
    V_AccountType=ARGV[3]
end

local AccountInfoValue=redis.call('hget', K_CardId, K_MembershipCardAccountId)
if(AccountInfoValue)
then
    returnValue=1--已创建账户
else
    returnValue=0--未创建账户

    --本金，透支
    redis.call('hset', K_StoredValueAccountTypesId, V_AccountType, K_MembershipCardAccountId)
    if(K_OverdraftDefineAmountId)
    then
        --透支额度
        redis.call('hset', K_OverdraftDefineAmountId, K_MembershipCardAccountId, V_CreditLine)
    end
    --卡值余额
    redis.call('hset', K_CardId, K_MembershipCardAccountId, V_AccountValue)

    if(K_OverdraftDefineAmountId)
    then
        V_AccountValue = 0
    end
    --卡总额
    local totalAmount = redis.call('hget', K_CardId, 'Total')
    if (totalAmount)
    then
        redis.call('hset', K_CardId, 'Total', string.format('%.8f',(totalAmount + V_AccountValue)))
    else
        redis.call('hset', K_CardId, 'Total', V_AccountValue)
    end
end

return { returnValue }
";

        /// <summary>
        /// 创建卡值,积分账户
        /// </summary>
        public const string CREATE_ACCOUNT = @"
local returnValue = nil

--获取卡值的卡id
local K_CardId = nil

--获取卡值的账户类型id
local K_MembershipCardAccountId = nil

--余额
local V_AccountValue = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    K_MembershipCardAccountId=KEYS[2]
end
if(ARGV[1])
then
    V_AccountValue=ARGV[1]
end

local AccountInfoValue=redis.call('hget', K_CardId, K_MembershipCardAccountId)
if(AccountInfoValue)
then
    returnValue=1--已创建账户
else
    returnValue=0--未创建账户

    --卡值余额
    redis.call('hset', K_CardId, K_MembershipCardAccountId, V_AccountValue)

    --卡总额
    local totalAmount = redis.call('hget', K_CardId, 'Total')
    if (totalAmount)
    then
        redis.call('hset', K_CardId, 'Total', string.format('%.8f',(totalAmount + V_AccountValue)))
    else
        redis.call('hset', K_CardId, 'Total', V_AccountValue)
    end
end

return { returnValue }
";

        #endregion


        /// <summary>
        /// 获取某卡的某个预授权号的值，如果值为空，则代表预授权不存在
        /// </summary>
        public const string GET_AUTHORIZATION_VALUE = @"
local returnValue = nil
local K_CardId = nil
local F_ApprovalCode = nil

if(KEYS[1])
then
    K_CardId=KEYS[1]
end
if(KEYS[2])
then
    F_ApprovalCode=KEYS[2]
end

-- 获取账户id
local authAmount = redis.call('HGET', K_CardId, F_ApprovalCode)
if(authAmount)
then
    --authAmount 非空
    returnValue = authAmount
end

return { returnValue }
";

        #region AUTH
        /// <summary>
        /// 获取卡预授权总金额
        /// </summary>
        public const string GET_AUTH_TOTAL = @"
local K_Auth = KEYS[1]

--预授权总金额
local totalAuthAmount = redis.call('GET', K_Auth) 
if(totalAuthAmount)
then
    totalAuthAmount = totalAuthAmount
else
    totalAuthAmount = 0
end

return { totalAuthAmount }
";

        public const string CANCEL_AUTH = @"
local K_Auth = KEYS[1]
local K_AuthTotal = KEYS[2]
local F_ApprovalCode = KEYS[3]

local authAmount = redis.call('HGET', K_Auth, F_ApprovalCode)
if(authAmount)
then
    if(redis.call('HDEL', K_Auth, F_ApprovalCode) ~= 0)
    then
        local totalAuthAmount = redis.call('GET', K_AuthTotal)
        if(totalAuthAmount)
        then
            if(totalAuthAmount - authAmount >= 0)
            then
                redis.call('INCRBYFLOAT', K_AuthTotal, -authAmount)
                return { 'OK' }
            end
            return { 'ERROR', 'Insufficient Auth Amount' }
        end  
        return { 'ERROR', 'Not exists total auth in redis' }     
    end
    return { 'ERROR', 'Cancel Failed' }
end
return { 'ERROR', 'Not exists auth in redis'}
";

        public const string APPEND_AUTH = @"
local K_CardId = KEYS[1]
local K_Auth = KEYS[2]
local K_AuthTotal = KEYS[3]
local F_ApprovalCode = KEYS[4]
local V_Amount = ARGV[1]

local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'
local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'
local overdraftAccountId = redis.call('HGET',keyCardStoredType,'OverdraftAccount')
local overdraftBalance = 0
--如果存在透支账户
if(overdraftAccountId)
then
    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('HGET', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalance)
    then
        overdraftBalance = overdraftBalanceTemp
    end
end

local totalAccountValue = redis.call('HGET', keyCardIdStoredValueAccounts, 'Total')
if(totalAccountValue)
then     
    local authAmount = redis.call('HGET', K_Auth, F_ApprovalCode)  
    if(authAmount)
    then
        local totalAuthAmount = redis.call('GET', K_AuthTotal) 
        if(totalAuthAmount)
        then
            if(totalAccountValue - totalAuthAmount - V_Amount + overdraftBalance >= 0)
            then
                local returnAmount = redis.call('HINCRBYFLOAT', K_Auth, F_ApprovalCode, V_Amount)
                redis.call('INCRBYFLOAT', K_AuthTotal, V_Amount)
                return { 'OK', returnAmount }
            end
            return { 'ERROR', 'Insufficient balance'} 
        end 
        return { 'ERROR', 'Not exists total auth in redis'}       
    end
    return { 'ERROR', 'Not exists auth in redis'}
end
return { 'ERROR', 'Not exists total amount'}
";

        public const string CREATE_AUTH = @"
local K_CardId = KEYS[1]
local K_Auth = KEYS[2]
local K_AuthTotal = KEYS[3]
local V_Amount = ARGV[1]
local V_Seed = ARGV[2]

local function generateRandomNumber()
    local a = ''
    math.randomseed(V_Seed)
    for i = 0,5,1 do
        a = a .. math.random(0,9)
    end
    return a
end

local function getApprovalCode()
    local number = generateRandomNumber()
    if(redis.call('HGET', K_Auth, number))
    then
        return getApprovalCode()
    else
        return number
    end
end

local keyCardIdStoredValueAccounts = 'LPS:Card:' ..K_CardId.. ':StoredValueAccounts'
local keyCardStoredType = 'LPS:Card:' ..K_CardId.. ':StoredValueAccountTypes'
local overdraftAccountId = redis.call('HGET',keyCardStoredType,'OverdraftAccount')
local overdraftBalance = 0
--如果存在透支账户
if(overdraftAccountId)
then
    --获取透支账户余额
    local overdraftBalanceTemp = redis.call('HGET', keyCardIdStoredValueAccounts, overdraftAccountId)
    if(overdraftBalance)
    then
        overdraftBalance = overdraftBalanceTemp
    end
end

local accountValue = redis.call('HGET', keyCardIdStoredValueAccounts, 'Total')
if(accountValue)
then     
    local totalAuthAmount = redis.call('GET', K_AuthTotal) 
    if(totalAuthAmount)
    then
        totalAuthAmount = totalAuthAmount
    else
        totalAuthAmount = 0
    end

    if(accountValue - totalAuthAmount - V_Amount + overdraftBalance >= 0)
    then
        local approvalCode = getApprovalCode()
        redis.call('HSET', K_Auth, approvalCode, V_Amount)
        redis.call('INCRBYFLOAT', K_AuthTotal, V_Amount)
        return { 'OK', approvalCode }
    end
    return { 'ERROR', 'Insufficient balance'}
end
return { 'ERROR', 'Not exists total amount'}";
        #endregion

        #region 卡号段
        // 查询是否有该卡号段的卡号池
        // 如果没有新建一个卡号段从Begin开始
        // 如果有卡号池取当前最大值
        public const string GET_MEMBERSHIP_CARD_NUMBER_LUA_SCRIPT = @"
local returnValue = nil

local key = KEYS[1]
local hashKey = KEYS[2]
local numberBegin = ARGV[1]
local numberEnd = ARGV[2]

local isExists = redis.call('HEXISTS', key, hashKey)
if (isExists == 1)
then
    local currentNumber = redis.call('HGET', key, hashKey)
    if(tonumber(currentNumber) <= tonumber(numberEnd))
    then
        returnValue = currentNumber
    else
        return { 'ERROR', '起始数字大于截至数字' }
    end    
else
    redis.call('HSET', key, hashKey, numberBegin)
    returnValue = numberBegin
end
redis.call('HINCRBY', key, hashKey, 1)
return { 'OK', returnValue }
";

        public const string GET_BATCH_MEMBERSHIP_CARD_NUMBER_LUA_SCRIPT = @"
local returnBegin = nil
local returnEnd = nil

local key = KEYS[1]
local hashKey = KEYS[2]
local numberBegin = ARGV[1]
local numberEnd = ARGV[2]
local buildNumber = ARGV[3] - tonumber(1)

local isExists = redis.call('HEXISTS', key, hashKey)
if (isExists == 1)
then
    local currentNumber = redis.call('HGET', key, hashKey)
    if(tonumber(currentNumber) + tonumber(buildNumber) <= tonumber(numberEnd))
    then
        returnBegin = currentNumber
        returnEnd = redis.call('HINCRBY', key, hashKey, buildNumber)
    else
        return { 'ERROR', '截至卡号大于截至数字' }
    end    
else
    redis.call('HSET', key, hashKey, numberBegin)
    returnBegin = numberBegin
    returnEnd = redis.call('HINCRBY', key, hashKey, buildNumber)
end
redis.call('HINCRBY', key, hashKey, 1)
return { 'OK', returnBegin, returnEnd }
";
        #endregion
    }
}
