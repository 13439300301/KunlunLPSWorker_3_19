using Kunlun.LPS.Worker.Core.Redis;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    /// <summary>
    /// 从redis获取或更新信息的服务
    /// </summary>
    public class GetOrUpdateInfoFromRedisService : IGetOrUpdateInfoFromRedisService
    {
        private readonly RedisClient _redis;
        private readonly ILogger<GetOrUpdateInfoFromRedisService> _logger;

        public GetOrUpdateInfoFromRedisService(RedisClient redis,
           ILogger<GetOrUpdateInfoFromRedisService> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        /// <summary>
        /// 判断是否存在预授权号
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approvalCode"></param>
        /// <returns></returns>
        public bool IsExistAuthByApprovalCode(long cardId, string approvalCode)
        {
            var db = _redis.Database;

            string keyCardIdAuthorization = $"LPS:Card:{cardId}:{RedisLuaScript.AUTHORIZATION}";

            var authAmount = (string[])db.ScriptEvaluate(RedisLuaScript.GET_AUTHORIZATION_VALUE,
            keys: new RedisKey[] { keyCardIdAuthorization, approvalCode });

            if (authAmount.Length == 0 || authAmount[0] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 变更储值账户余额，增减皆可（会员卡不存在透支账户的情况下）
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public string[] ChangeStoredValueAccountBalance(long cardId, string jsonStr)
        {
            var db = _redis.Database;

            string keyCardId = $"{cardId}";
            var aa = (string[])db.ScriptEvaluate(
                    RedisLuaScript.CHANGE_STOREDVALUE_ACCOUNT_BALANCE,
                    keys: new RedisKey[] { keyCardId },
                    values: new RedisValue[] { jsonStr });

            return aa;
        }

        /// <summary>
        /// 增加储值账户余额，按账户（会员卡存在透支账户的情况下）
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public string[] AddStoredValueAccountBalanceExistOverdraft(long cardId, string jsonStr)
        {
            var db = _redis.Database;

            string keyCardId = $"{cardId}";
            var aa = (string[])db.ScriptEvaluate(
                    RedisLuaScript.ADD_STOREDVALUE_ACCOUNT_BALANCE_EXIST_OVERDRAFT,
                    keys: new RedisKey[] { keyCardId },
                    values: new RedisValue[] { jsonStr });

            return aa;
        }

        /// <summary>
        /// 按账户扣减账户余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public string[] DeductAccountBalance(long cardId, string jsonStr)
        {
            var db = _redis.Database;

            string keyCardId = $"{cardId}";
            var aa = (string[])db.ScriptEvaluate(
                    RedisLuaScript.DEDUCT_ACCOUNT_BALANCE,
                    keys: new RedisKey[] { keyCardId },
                    values: new RedisValue[] { jsonStr });

            return aa;
        }

        /// <summary>
        /// 获取redis中的账户信息
        /// </summary>
        /// <param name="cardId">卡Id</param>
        /// <param name="membershipCardAccountId">账户类型Id</param>
        /// <param name="accountType">账户类型（储值，积分，成长值）</param>
        public string[] GetRedisAccountInfo(long cardId, long membershipCardAccountId, string accountType)
        {
            var db = _redis.Database;

            //会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";
            string fieldMembershipCardAccountId = $"{membershipCardAccountId}";

            //获取redis里的账户余额
            var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.GET_ACCOUNT_VALUE,
            keys: new RedisKey[] { keyCardId, fieldMembershipCardAccountId });

            return accountRedisValue;

        }
        public string[] GetRedisCardTotalAmount(long cardId, string accountType)
        {
            var db = _redis.Database;

            //会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";

            //获取redis里的卡总余额
            var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.GET_ACCOUNT_VALUE,
            keys: new RedisKey[] { keyCardId, "Total" });

            return accountRedisValue;
        }

        /// <summary>
        /// 获取账户的哈希值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="fieid">fieid</param>
        /// <returns></returns>
        public string[] GetRedisAccountHash(string key, string fieid)
        {
            var db = _redis.Database;
            var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.GET_ACCOUNT_VALUE,
            keys: new RedisKey[] { key, fieid });
            if (accountRedisValue.Length > 0 && accountRedisValue?[0] != null)
            {
                return accountRedisValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取卡的透支余额和透支额度
        /// </summary>
        /// <param name="cardId">卡id</param>
        /// <returns></returns>
        public string[] GetRedisOverdraft(long cardId)
        {
            var db = _redis.Database;
            var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.GET_CARD_ACCOUNT_OVERDRAFT,
            keys: new RedisKey[] { cardId.ToString() });
            if (accountRedisValue?[0] != null)
            {
                return accountRedisValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新账户余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="membershipCardAccountId"></param>
        /// <param name="accountValue"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public bool UpdateRedisAccountBalance(long cardId, long membershipCardAccountId, decimal accountValue, string accountType)
        {
            var db = _redis.Database;

            //账户类型:会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";  //列表的key不能为整型
            string fieldMembershipCardAccountId = $"{membershipCardAccountId}";

            string keyCardOverdraft = $"LPS:Card:{cardId}:{RedisLuaScript.STORED_VALUE_ACCOUNT_TYPE}";

            //更新redis里的账户余额
            var accountRedis = (string[])db.ScriptEvaluate(RedisLuaScript.UPDATE_ACCOUNT_BALANCE,
            keys: new RedisKey[] { keyCardId, fieldMembershipCardAccountId, keyCardOverdraft },
            values: new RedisValue[] { Convert.ToDouble(accountValue) });
            if (accountRedis.Length == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新本金和透支账户类型对应的账户id
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="membershipCardAccountId"></param>
        /// <param name="storedValueAccountType">固定值“OverdraftAccount”或“PrincipalAccount”</param>
        /// <returns></returns>
        public bool UpdateRedisStoredValueAccountType(long cardId, long membershipCardAccountId, string storedValueAccountType)
        {
            var db = _redis.Database;

            //账户类型:会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{RedisLuaScript.STORED_VALUE_ACCOUNT_TYPE}";  //列表的key不能为整型
            string fieldStoredValueAccountType = $"{storedValueAccountType}";

            //更新redis里的账户余额
            var accountRedis = (string[])db.ScriptEvaluate(RedisLuaScript.UPDATE_STORED_VALUE_ACCOUNT_TYPE,
            keys: new RedisKey[] { keyCardId, fieldStoredValueAccountType },
            values: new RedisValue[] { membershipCardAccountId });
            if (accountRedis.Length == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新透支账户的定义额度
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="membershipCardAccountId"></param>
        /// <param name="overdraftDefineAmount"></param>
        /// <returns></returns>
        public bool UpdateRedisOverdraftAccountDefineAmount(long cardId, long membershipCardAccountId, decimal overdraftDefineAmount)
        {
            var db = _redis.Database;

            //账户类型:会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{RedisLuaScript.OVERDRAFT_DEFINE_AMOUNT}";  //列表的key不能为整型
            string fieldMembershipCardAccountId = $"{membershipCardAccountId}";

            //更新redis里的账户余额
            var accountRedis = (string[])db.ScriptEvaluate(RedisLuaScript.UPDATE_OVERDRAFT_DEFINE_AMOUNT,
            keys: new RedisKey[] { keyCardId, fieldMembershipCardAccountId },
            values: new RedisValue[] { Convert.ToDouble(overdraftDefineAmount) });
            if (accountRedis.Length == 0)
            {
                return false;
            }
            return true;
        }
        public string[] CancelAuthorization(long currentCardId, long? mainCardId, string approvalCode)
        {
            var db = _redis.Database;

            string keyAuth = $"LPS:Card:{currentCardId}:{RedisLuaScript.AUTHORIZATION}";
            var cardId = mainCardId.HasValue ? mainCardId : currentCardId;
            string keyAuthTotal = $"LPS:Card:{cardId}:{RedisLuaScript.AUTHORIZATION_TOTAL}";

            var luaResult = (string[])db.ScriptEvaluate(RedisLuaScript.CANCEL_AUTH,
                    keys: new RedisKey[] { keyAuth, keyAuthTotal, approvalCode });

            return luaResult;
        }

        public string[] AppendAuthorization(long currentCardId, long? mainCardId, string approvalCode, decimal amount)
        {
            var db = _redis.Database;
            string keyAuth = $"LPS:Card:{currentCardId}:{RedisLuaScript.AUTHORIZATION}";
            var cardId = mainCardId.HasValue ? mainCardId : currentCardId;
            string keyAuthTotal = $"LPS:Card:{cardId}:{RedisLuaScript.AUTHORIZATION_TOTAL}";

            var luaResult = (string[])db.ScriptEvaluate(RedisLuaScript.APPEND_AUTH,
                    keys: new RedisKey[] { cardId.ToString(), keyAuth, keyAuthTotal, approvalCode },
                    values: new RedisValue[] { Convert.ToDouble(amount) });

            return luaResult;
        }

        public string[] CreateAuthorization(long currentCardId, long? mainCardId, decimal amount)
        {
            var db = _redis.Database;
            string keyAuth = $"LPS:Card:{currentCardId}:{RedisLuaScript.AUTHORIZATION}";
            var cardId = mainCardId.HasValue ? mainCardId : currentCardId;
            string keyAuthTotal = $"LPS:Card:{cardId}:{RedisLuaScript.AUTHORIZATION_TOTAL}";

            var luaResult = (string[])db.ScriptEvaluate(RedisLuaScript.CREATE_AUTH,
                    keys: new RedisKey[] { cardId.ToString(), keyAuth, keyAuthTotal },
                    values: new RedisValue[] { Convert.ToDouble(amount), Guid.NewGuid().GetHashCode() });

            return luaResult;
        }

        /// <summary>
        /// 计算并更新积分账户余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="membershipCardAccountId"></param>
        /// <param name="transactionPoint"></param>
        /// <returns></returns>
        public string[] CalculateAndUpdatePointAccountBalance(long cardId, long membershipCardAccountId, decimal transactionPoint)
        {
            var db = _redis.Database;
            string keyCardId = $"LPS:Card:{cardId}:{RedisLuaScript.ACCOUNT_TYPE_POINTS}";
            var luaResult = (string[])db.ScriptEvaluate(
                    RedisLuaScript.CALCULATE_UPDATE_POINT_ACCOUNT,
                    keys: new RedisKey[] { keyCardId },
                    values: new RedisValue[] { membershipCardAccountId.ToString(), Convert.ToDouble(transactionPoint) });

            return luaResult;
        }

        /// <summary>
        /// 计算并更新成长值账户余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="membershipCardAccountId"></param>
        /// <param name="transactionPoint"></param>
        /// <returns></returns>
        public string[] CalculateAndUpdateGrowthAccountBalance(long cardId, long membershipCardAccountId, decimal transactionGrowth)
        {
            var db = _redis.Database;
            string keyCardId = $"LPS:Card:{cardId}:{RedisLuaScript.ACCOUNT_TYPE_GROWTH}";
            var luaResult = (string[])db.ScriptEvaluate(
                    RedisLuaScript.CALCULATE_UPDATE_GROWTH_ACCOUNT,
                    keys: new RedisKey[] { keyCardId },
                    values: new RedisValue[] { membershipCardAccountId.ToString(), Convert.ToDouble(transactionGrowth) });

            return luaResult;
        }

        /// <summary>
        /// 扣减卡值，一般用于支付
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <param name="transactionAmount"></param>
        /// <param name="approvalCode"></param>
        /// <returns></returns>
        public string[] DeductCardFee(long cardId, string jsonStr, decimal transactionAmount, string approvalCode = "")
        {
            var db = _redis.Database;
            string keyCardId = $"{cardId}";
            var luaResult = (string[])db.ScriptEvaluate(
                    RedisLuaScript.DEDUCT_CARD_FEE,
                    keys: new RedisKey[] { keyCardId, approvalCode },
                    values: new RedisValue[] { jsonStr, Convert.ToDouble(transactionAmount) });
            return luaResult;
        }

        public string[] GetCardAuthTotalAmount(long cardId)
        {
            var db = _redis.Database;
            string keyCardId = $"LPS:Card:{cardId}:{RedisLuaScript.AUTHORIZATION_TOTAL}";
            var luaResult = (string[])db.ScriptEvaluate(
                    RedisLuaScript.GET_AUTH_TOTAL,
                    keys: new RedisKey[] { keyCardId });

            return luaResult;
        }

        public string[] AdjustOverdraftAccountDefineAmount(long cardId, decimal newDefineAmount)
        {
            var db = _redis.Database;
            var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.ADJUST_OVERDRAFT_ACCOUNT_DEFINE_AMOUNT,
            keys: new RedisKey[] { cardId.ToString() },
            values: new RedisValue[] { Convert.ToDouble(newDefineAmount) });
            if (accountRedisValue?[0] != null)
            {
                return accountRedisValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 生成账户
        /// </summary>
        /// <param name="cardId">卡id</param>
        /// <param name="accountTypeId">账户类型id</param>
        /// <param name="accountType">账户类型</param>
        /// <param name="overdraftAccount">透支或本金</param>
        /// <returns>1:已创建 0:未创建</returns>
        public string CreateAccount(string cardId, string accountTypeId, string accountType, string overdraftAccount, string value, string creditLine)
        {
            var db = _redis.Database;
            string returnStr = "";
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";
            string keyCardOverdraft = $"LPS:Card:{cardId}:{RedisLuaScript.STORED_VALUE_ACCOUNT_TYPE}";
            if (overdraftAccount == "OverdraftAccount")
            {
                //透支
                string keyCardOverdraftAmount = $"LPS:Card:{cardId}:{RedisLuaScript.OVERDRAFT_DEFINE_AMOUNT}";
                var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.CREATE_ACCOUNT_STOREDVALUEACCOUNTTYPES,
keys: new RedisKey[] { keyCardId, accountTypeId, keyCardOverdraft, keyCardOverdraftAmount },
values: new RedisValue[] { value, creditLine, "OverdraftAccount" });
                if (accountRedisValue.Length > 0 && accountRedisValue[0] != null)
                {
                    returnStr = accountRedisValue[0];
                }
            }
            else if (overdraftAccount == "PrincipalAccount")
            {
                //本金，不传额度id
                var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.CREATE_ACCOUNT_STOREDVALUEACCOUNTTYPES,
keys: new RedisKey[] { keyCardId, accountTypeId, keyCardOverdraft },
values: new RedisValue[] { value, 0, "PrincipalAccount" });
                if (accountRedisValue.Length > 0 && accountRedisValue[0] != null)
                {
                    returnStr = accountRedisValue[0];
                }
            }
            else
            {
                var accountRedisValue = (string[])db.ScriptEvaluate(RedisLuaScript.CREATE_ACCOUNT,
keys: new RedisKey[] { keyCardId, accountTypeId },
values: new RedisValue[] { value });
                if (accountRedisValue.Length > 0 && accountRedisValue[0] != null)
                {
                    returnStr = accountRedisValue[0];
                }
            }

            return returnStr;
        }

        public decimal GetRedisCardTotal(long cardId, string accountType)
        {
            //会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";

            var db = _redis.Database;
            //获取redis里的卡总余额
            var accountRedisValue = db.HashGet(keyCardId, "Total");
            if (accountRedisValue.HasValue)
            {
                return Convert.ToDecimal(accountRedisValue);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 积分池扣减
        /// </summary>
        /// <returns></returns>
        public string[] PointPoolBalance(string jsonStr, decimal transactionPoint)
        {
            var db = _redis.Database;
            var luaResult = (string[])db.ScriptEvaluate(
                    RedisLuaScript.CALCULATE_UPDATE_POINTPOOL_ACCOUNT,
                    keys: new RedisKey[] { jsonStr },
                    values: new RedisValue[] { Convert.ToDouble(transactionPoint) });
            return luaResult;
        }

        public int? GetCouponInventory(long couponTypeId)
        {
            var db = _redis.Database;
            //券类型Id
            string key = $"LPS:Coupon:{couponTypeId}:Inventory";
            var count = db.StringGet(key);
            if (count.HasValue)
            {
                return Convert.ToInt32(count);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更改库存
        /// </summary>
        /// <param name="couponTypeId"></param>
        /// <param name="Inventory"></param>
        public void UpdateRedisCouponInventory(long couponTypeId, int Inventory)
        {
            var db = _redis.Database;
            //券类型Id
            string key = $"LPS:Coupon:{couponTypeId}:Inventory";
            db.StringIncrement(key, Inventory);
        }

        /// <summary>
        /// 是否存在redis中的账户信息
        /// </summary>
        /// <param name="cardId">卡Id</param>
        /// <param name="membershipCardAccountId">账户类型Id</param>
        /// <param name="accountType">账户类型（储值，积分）</param>
        public bool IsExistRedisAccountInfo(long cardId, long membershipCardAccountId, string accountType)
        {
            var db = _redis.Database;
            //会员卡Id
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";
            string fieldMembershipCardAccountId = $"{membershipCardAccountId}";

            //获取redis里的账户余额
            var accountRedisValue = db.HashGet(keyCardId, fieldMembershipCardAccountId);

            return accountRedisValue.HasValue;
        }

        /// <summary>
        /// 删除账户
        /// </summary>
        /// <param name="cardId">卡id</param>
        /// <param name="accountTypeId">账户类型id</param>
        public void DeleteAccount(string cardId, string accountTypeId, string accountType)
        {
            var db = _redis.Database;
            _logger.LogInformation($"DeleteAccount删除");
            string keyCardId = $"LPS:Card:{cardId}:{accountType}";
            var accountValue = db.HashGet(keyCardId, accountTypeId);
            if (accountValue.HasValue)
            {
                _logger.LogInformation($"DeleteAccount删除{keyCardId}账户类型{accountTypeId}");
                db.HashDelete(keyCardId, accountTypeId);
                db.HashDelete(keyCardId, "Total");
            }
        }

        //public int? GetCouponInventory(long couponTypeId)
        //{
        //    var db = _redis.Database;
        //    //券类型Id
        //    string key = $"LPS:Coupon:{couponTypeId}:Inventory";
        //    var count = db.StringGet(key);
        //    if (count.HasValue)
        //    {
        //        return Convert.ToInt32(count);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
