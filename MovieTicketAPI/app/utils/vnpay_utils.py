import hashlib
import hmac
import urllib.parse
from datetime import datetime
import os

from dotenv import load_dotenv
load_dotenv()

VNPAY_URL = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
VNPAY_TMN_CODE = os.getenv("VNPAY_TMN_CODE", "YOUR_TMN_CODE")
VNPAY_HASH_SECRET = os.getenv("VNPAY_HASH_SECRET", "YOUR_HASH_SECRET")
VNPAY_RETURN_URL = "http://127.0.0.1:8000/api/v1/tickets/vnpay_return"  # Replace with your return URL

def __hmacsha512(key: str, data: str) -> str:
    """
    Generate HMAC SHA512 hash using VNPay's algorithm
    """
    byteKey = key.encode('utf-8')
    byteData = data.encode('utf-8')
    return hmac.new(byteKey, byteData, hashlib.sha512).hexdigest()

def generate_vnpay_payment_url(order_id: int, amount: int, order_info: str) -> str:
    """
    Generate a VNPay payment URL for the given order.
    """
    # Format current timestamp
    create_date = datetime.now().strftime('%Y%m%d%H%M%S')
    
    params = {
        "vnp_Version": "2.1.0",
        "vnp_Command": "pay",
        "vnp_TmnCode": VNPAY_TMN_CODE,
        "vnp_Amount": str(amount * 100),
        "vnp_CurrCode": "VND",
        "vnp_BankCode": "",
        "vnp_TxnRef": f"{create_date}_{order_id}",
        "vnp_OrderInfo": order_info,
        "vnp_OrderType": "billpayment",
        "vnp_Locale": "vn",
        "vnp_ReturnUrl": VNPAY_RETURN_URL,
        "vnp_IpAddr": "192.168.1.11",
        "vnp_CreateDate": create_date
    }

    # Sort parameters and create hash data (without URL encoding)
    sorted_params = sorted(params.items())
    hash_data = "&".join([f"{k}={str(v)}" for k, v in sorted_params])
    
    # Generate secure hash using VNPay's algorithm
    secure_hash = __hmacsha512(VNPAY_HASH_SECRET, hash_data)

    # Add secure hash to params
    params["vnp_SecureHash"] = secure_hash

    # Create final query string with URL encoding
    query_string = "&".join([f"{k}={urllib.parse.quote_plus(str(v))}" for k, v in sorted(params.items())])
    
    return f"{VNPAY_URL}?{query_string}"

def verify_vnpay_response(params: dict) -> bool:
    """
    Verify the VNPay response using their hashing algorithm
    """
    if "vnp_SecureHash" not in params:
        return False
        
    # Remove secure hash from params
    secure_hash = params.pop("vnp_SecureHash")
    
    # Sort remaining parameters
    sorted_params = sorted(params.items())
    
    # Create hash data without URL encoding
    hash_data = "&".join([f"{key}={str(value)}" for key, value in sorted_params])
    
    # Calculate secure hash
    calculated_hash = __hmacsha512(VNPAY_HASH_SECRET, hash_data)
    
    return calculated_hash == secure_hash