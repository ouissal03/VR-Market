/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class OrderSender : MonoBehaviour
{
 
    private string apiUrl = "http://192.168.137.75:5000/api/orders"; // Replace with your API URL

    
    private string userName = "ouissal";
    private string userEmail = "wissalmaarouf3@gmail.com";
    private string userPhone = "123456789";
    private string userAddress = "123 Maple Street";

    // Sample products (you will adjust quantities based on stock)
    private List<Product> products = new List<Product>()
    {
        new Product() { name = "Oil", quantity = 3 },   // Will check stock
        new Product() { name = "Milk", quantity = 3 }
    };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting OrderSender script...");
        StartCoroutine(CheckStockBeforeOrder());
    }

    // This method checks stock before sending the order
    IEnumerator CheckStockBeforeOrder()
    {
        Debug.Log("Checking stock before sending the order...");

        foreach (var product in products)
        {
            bool isStockAvailable = false;

            // Wait for the result of the stock check
            yield return StartCoroutine(CheckStockAvailability(product, (available) => {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError("Insufficient stock for product: " + product.name);
                yield break; // Exit early if stock is insufficient
            }
        }

        // If stock is available for all products, send the order
        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    // Method to check stock availability via API
    IEnumerator CheckStockAvailability(Product product, System.Action<bool> callback)
    {
        Debug.Log("Checking stock for product: " + product.name);

        // Construct the correct stock check URL
        
        string stockCheckUrl = "http://localhost:5000/api/products/checkStock/" + product.name + "?quantity=" + product.quantity;

        Debug.Log("Stock Check URL: " + stockCheckUrl);  // Debugging the URL

        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        // Wait for the request to complete
        yield return stockRequest.SendWebRequest();

        // Check if there was an error during the request
        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error during stock check request: " + stockRequest.error);
            callback(false);
            yield break;  // Exit the method if there was an error with the request
        }

        // Log the response text for debugging
        string responseText = stockRequest.downloadHandler.text;
        Debug.Log("Stock Check Response: " + responseText);  // Debugging the response

        try
        {
            // Parse the response (assuming the API returns a JSON response like { "stock": 5 })
            StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(responseText);

            if (stockResponse.stock >= product.quantity)
            {
                Debug.Log(product.name + " stock is sufficient.");
                callback(true); // Pass true if stock is sufficient
            }
            else
            {
                Debug.LogError("Insufficient stock for " + product.name + ". Available: " + stockResponse.stock);
                callback(false); // Pass false if stock is insufficient
            }
        }
        catch (Exception ex)
        {
            // If there's an error parsing the response, log the exception
            Debug.LogError("Error parsing stock check response: " + ex.Message);
            callback(false);
        }
    }



    // Create and send the order to the backend
    IEnumerator SendOrder()
    {
        // Create the order data
        Order order = new Order()
        {
            user = new User()
            {
                name = userName,
                email = userEmail,
                phone = userPhone,
                address = userAddress
            },
            products = products
        };

        // Convert the order data to JSON
        string jsonOrder = JsonConvert.SerializeObject(order);

        // Prepare the POST request
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending order: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    // Define the data structure for the order
    [System.Serializable]
    public class Order
    {
        public User user;
        public List<Product> products;
    }

    [System.Serializable]
    public class User
    {
        public string name;
        public string email;
        public string phone;
        public string address;
    }

    [System.Serializable]
    public class Product
    {
        public string name;
        public int quantity;
    }

    
    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}


CODE VERSION 2  

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class OrderSender : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiBaseUrl = "http://localhost:5000/api"; 

    [Header("User Details")]
    private string userName = "ouissal";
    private string userEmail = "wissalmaarouf3@gmail.com";
    private string userPhone = "123456789";
    private string userAddress = "123 Maple Street";

    [Header("Order Configuration")]
    public List<Product> products = new List<Product>()
    {
        new Product() { name = "Oil", quantity = 3 },
        new Product() { name = "Milk", quantity = 3 }
    };

    public bool checkStockBeforeOrder = true; // Toggle stock checking

    void Start()
    {
        Debug.Log("Starting OrderSender script...");

        if (checkStockBeforeOrder)
        {
            StartCoroutine(CheckStockBeforeOrder());
        }
        else
        {
            StartCoroutine(SendOrder());
        }
    }

    IEnumerator CheckStockBeforeOrder()
    {
        Debug.Log("Checking stock before sending the order...");

        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError($"Insufficient stock for product: {product.name}");
                yield break; // Exit early if stock is insufficient
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(Product product, Action<bool> callback)
    {
        string stockCheckUrl = $"{apiBaseUrl}/products/checkStock/{product.name}?quantity={product.quantity}";
        Debug.Log($"Checking stock for product: {product.name} (URL: {stockCheckUrl})");

        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);
        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error checking stock for {product.name}: {stockRequest.error}");
            callback(false);
            yield break;
        }

        try
        {
            StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
            bool isAvailable = stockResponse != null && stockResponse.stock >= product.quantity;

            Debug.Log(isAvailable
                ? $"{product.name} stock is sufficient."
                : $"Insufficient stock for {product.name}. Available: {stockResponse?.stock}");

            callback(isAvailable);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing stock response for {product.name}: {ex.Message}");
            callback(false);
        }
    }

    IEnumerator SendOrder()
    {
        Order order = new Order
        {
            user = new User
            {
                name = userName,
                email = userEmail,
                phone = userPhone,
                address = userAddress
            },
            products = products
        };

        string jsonOrder = JsonConvert.SerializeObject(order);
        string orderUrl = $"{apiBaseUrl}/orders";

        UnityWebRequest request = new UnityWebRequest(orderUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonOrder)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
            Debug.Log($"Response: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError("Error sending order: " + request.error);
            Debug.LogError($"Response Code: {request.responseCode}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }
    }

    [Serializable]
    public class Order
    {
        public User user;
        public List<Product> products;
    }

    [Serializable]
    public class User
    {
        public string name;
        public string email;
        public string phone;
        public string address;
    }

    [Serializable]
    public class Product
    {
        public string name;
        public int quantity;
    }

    [Serializable]
    public class StockResponse
    {
        public int stock;
    }
}
*/

/*using UnityEngine;
using UnityEngine.UI; // Import for UI components
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class OrderSender : MonoBehaviour
{
    // API URL
    private string apiUrl = "http://192.168.137.75:5000/api/orders"; 

    // Sample user and product data
    private string userName = "test casque";
    private string userEmail = "vr@example.com";
    private string userPhone = "123456789";
    private string userAddress = "123 Maple Street";

    private List<Product> products = new List<Product>()
    {
        new Product() { name = "Oil", quantity = 3 },
        new Product() { name = "Milk", quantity = 3 }
    };

    // Reference to the Send Order button
    public Button sendOrderButton;

    void Start()
    {
        // Log script initialization
        Debug.Log("OrderSender script initialized.");

        // Attach the button click listener
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(OnSendOrderButtonClick);
        }
        else
        {
            Debug.LogError("Send Order button is not assigned in the inspector!");
        }
    }

    // Method triggered on button click
    public void OnSendOrderButtonClick()
    {
        Debug.Log("Send Order button clicked!");
        StartCoroutine(CheckStockBeforeOrder());
    }

    IEnumerator CheckStockBeforeOrder()
    {
        Debug.Log("Checking stock before sending the order...");

        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError("Insufficient stock for product: " + product.name);
                yield break;
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(Product product, Action<bool> callback)
    {
        string stockCheckUrl = "http://192.168.137.75:5000/api/products/checkStock/" + product.name + "?quantity=" + product.quantity;
        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error during stock check request: " + stockRequest.error);
            callback(false);
            yield break;
        }

        try
        {
            StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
            callback(stockResponse.stock >= product.quantity);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing stock check response: " + ex.Message);
            callback(false);
        }
    }

    IEnumerator SendOrder()
    {
        Order order = new Order()
        {
            user = new User()
            {
                name = userName,
                email = userEmail,
                phone = userPhone,
                address = userAddress
            },
            products = products
        };

        string jsonOrder = JsonConvert.SerializeObject(order);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending order: " + request.error);
        }
    }

    // Data classes
    [System.Serializable]
    public class Order
    {
        public User user;
        public List<Product> products;
    }

    [System.Serializable]
    public class User
    {
        public string name;
        public string email;
        public string phone;
        public string address;
    }

    [System.Serializable]
    public class Product
    {
        public string name;
        public int quantity;
    }

    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}*/

/*
''''''''''''''''''''''''''''''''''''''''''''
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.XR.Interaction.Toolkit;

public class OrderSender : MonoBehaviour
{
    // API URL
    private string apiUrl = "http://192.168.137.75:5000/api/orders"; // Use your laptop's IP

    // Sample user and product data
    private string userName = "test casque";
    private string userEmail = "vr@example.com";
    private string userPhone = "123456789";
    private string userAddress = "123 Maple Street";

    private List<Product> products = new List<Product>()
    {
        new Product() { name = "Oil", quantity = 3 },
        new Product() { name = "Milk", quantity = 3 }
    };

    // Reference to the Send Order button (UI Button, NOT XR Button)
    public Button sendOrderButton;

    void Start()
    {
        // Log script initialization
        Debug.Log("OrderSender script initialized.");

        // Ensure the XR button can interact
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(OnSendOrderButtonClick);
        }
        else
        {
            Debug.LogError("Send Order button is not assigned in the inspector!");
        }
    }

    // Method triggered on button press in VR
    public void OnSendOrderButtonClick()
    {
        Debug.Log("Send Order button clicked!");
        StartCoroutine(CheckStockBeforeOrder());

        
        
    }

    IEnumerator CheckStockBeforeOrder()
    {
        Debug.Log("Checking stock before sending the order...");

        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError("Insufficient stock for product: " + product.name);
                yield break;
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(Product product, System.Action<bool> callback)
    {
        string stockCheckUrl = "http://192.168.137.75:5000/api/products/checkStock/" + product.name + "?quantity=" + product.quantity;
        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error during stock check request: " + stockRequest.error);
            callback(false);
            yield break;
        }

        try
        {
            StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
            callback(stockResponse.stock >= product.quantity);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing stock check response: " + ex.Message);
            callback(false);
        }
    }

    IEnumerator SendOrder()
    {
        Order order = new Order()
        {
            user = new User()
            {
                name = userName,
                email = userEmail,
                phone = userPhone,
                address = userAddress
            },
            products = products
        };

        string jsonOrder = JsonConvert.SerializeObject(order);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending order: " + request.error);
        }
    }

    // Data classes
    [System.Serializable]
    public class Order
    {
        public User user;
        public List<Product> products;
    }

    [System.Serializable]
    public class User
    {
        public string name;
        public string email;
        public string phone;
        public string address;
    }

    [System.Serializable]
    public class Product
    {
        public string name;
        public int quantity;
    }

    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}
''''''''''''''''''''''''''''''''''''''''''''''''
*/

/*
************************************************
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.XR.Interaction.Toolkit;

public class OrderSender : MonoBehaviour
{
    // API URL
    private string apiUrl = "http://192.168.137.75:5000/api/orders"; 

    // Sample user and product data
    private string userName = "test casque";
    private string userEmail = "vr@example.com";
    private string userPhone = "123456789";
    private string userAddress = "123 Maple Street";

    private List<Product> products = new List<Product>()
    {
        new Product() { name = "Oil", quantity = 3 },
        new Product() { name = "Milk", quantity = 3 }
    };

    // Reference to the Send Order button (UI Button, NOT XR Button)
    public Button sendOrderButton;

    void Start()
    {
        // Log script initialization
        Debug.Log("OrderSender script initialized.");

        // Ensure the XR button can interact
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(OnSendOrderButtonClick);
        }
        else
        {
            Debug.LogError("Send Order button is not assigned in the inspector!");
        }
    }

    // Method triggered on button press in VR
    public void OnSendOrderButtonClick()
    {
        Debug.Log("Send Order button clicked!");
        StartCoroutine(CheckStockBeforeOrder());
    }

    IEnumerator CheckStockBeforeOrder()
    {
        Debug.Log("Checking stock before sending the order...");

        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError("Insufficient stock for product: " + product.name);
                yield break;
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(Product product, System.Action<bool> callback)
    {
        string stockCheckUrl = "http://192.168.137.75:5000/api/products/checkStock/" + product.name + "?quantity=" + product.quantity;
        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        // Bypass SSL certificate validation for local testing
        stockRequest.certificateHandler = new BypassCertificateHandler();

        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error during stock check request: " + stockRequest.error);
            callback(false);
            yield break;
        }

        try
        {
            StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
            callback(stockResponse.stock >= product.quantity);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing stock check response: " + ex.Message);
            callback(false);
        }
    }

    IEnumerator SendOrder()
    {
        Order order = new Order()
        {
            user = new User()
            {
                name = userName,
                email = userEmail,
                phone = userPhone,
                address = userAddress
            },
            products = products
        };

        string jsonOrder = JsonConvert.SerializeObject(order);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Bypass SSL certificate validation for local testing
        request.certificateHandler = new BypassCertificateHandler();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending order: " + request.error);
        }
    }

    // Data classes
    [System.Serializable]
    public class Order
    {
        public User user;
        public List<Product> products;
    }

    [System.Serializable]
    public class User
    {
        public string name;
        public string email;
        public string phone;
        public string address;
    }

    [System.Serializable]
    public class Product
    {
        public string name;
        public int quantity;
    }

    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}
*/

/*
'''''''''''''''''''''''''''''''''''''''''''''''''''''
using UnityEngine;
using UnityEngine.UI; // Import for UI components
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.XR.Interaction.Toolkit;

public class OrderSender : MonoBehaviour
{
    // API URL
    private string apiUrl = "http://192.168.137.75:5000/api/orders";

    // Sample user and product data
    private string userName = "casque again";
    private string userEmail = "vr@example.com";
    private string userPhone = "123456789";
    private string userAddress = "123 Maple Street";

    private List<Product> products = new List<Product>()
    {
        new Product() { name = "Oil", quantity = 3 },
        new Product() { name = "Milk", quantity = 3 }
    };

    // Reference to the Send Order button (UI Button, NOT XR Button)
    public Button sendOrderButton;

    void Start()
    {
        // Log script initialization
        Debug.Log("OrderSender script initialized.");

        // Ensure the XR button can interact
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(OnSendOrderButtonClick);
        }
        else
        {
            Debug.LogError("Send Order button is not assigned in the inspector!");
        }
    }

    // Method triggered on button press in VR
    public void OnSendOrderButtonClick()
    {
        Debug.Log("Send Order button clicked!");
        StartCoroutine(CheckStockBeforeOrder());
    }

    IEnumerator CheckStockBeforeOrder()
    {
        Debug.Log("Checking stock before sending the order...");

        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError("Insufficient stock for product: " + product.name);
                yield break;
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(Product product, System.Action<bool> callback)
    {
        string stockCheckUrl = "http://192.168.137.75:5000/api/products/checkStock/" + product.name + "?quantity=" + product.quantity;
        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error during stock check request: " + stockRequest.error);
            callback(false);
            yield break;
        }

        try
        {
            StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
            callback(stockResponse.stock >= product.quantity);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing stock check response: " + ex.Message);
            callback(false);
        }
    }

    IEnumerator SendOrder()
    {
        Order order = new Order()
        {
            user = new User()
            {
                name = userName,
                email = userEmail,
                phone = userPhone,
                address = userAddress
            },
            products = products 
        };

        // Serialize the order object to JSON
        string jsonOrder = JsonConvert.SerializeObject(order);

        // Debug the serialized JSON to ensure it's correct
        Debug.Log("Sending Order JSON: " + jsonOrder);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");

        // Convert the JSON string to byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the Content-Type header to application/json
        request.SetRequestHeader("Content-Type", "application/json");

        // Optional: Bypass SSL certificate validation (for local testing)
        request.certificateHandler = new BypassCertificateHandler();

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending order: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    // Data classes
    [System.Serializable]
    public class Order
    {
        public User user;
        public List<Product> products;
    }

    [System.Serializable]
    public class User
    {
        public string name;
        public string email;
        public string phone;
        public string address;
    }

    [System.Serializable]
    public class Product
    {
        public string name;
        public int quantity;
    }

    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}
*/

/* new script with cart integration*/

/* last working script */

/*
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class OrderSender : MonoBehaviour
{
    private string apiUrl = "http://192.168.137.75:5000/api/orders";
    
    public Cart cart; // Reference to the Cart
    public Button sendOrderButton;

    void Start()
    {
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(OnSendOrderButtonClick);
        }
        else
        {
            Debug.LogError("Send Order button is not assigned in the inspector!");
        }
    }

    public void OnSendOrderButtonClick()
    {
        Debug.Log("Send Order button clicked!");
        PrepareOrderFromCart();
        StartCoroutine(CheckStockBeforeOrder());
    }

    void PrepareOrderFromCart()
    {
        if (cart != null)
        {
            List<Product> cartProducts = cart.GetProducts();
            UpdateProductsFromCart(cartProducts);
        }
        else
        {
            Debug.LogError("Cart is not assigned!");
        }
    }

    private List<ProductData> products = new List<ProductData>();

    void UpdateProductsFromCart(List<Product> cartProducts)
    {
        products.Clear();
        foreach (Product product in cartProducts)
        {
            products.Add(new ProductData { name = product.name, quantity = product.quantity });
        }
        Debug.Log("Updated product list from cart.");
    }

    IEnumerator CheckStockBeforeOrder()
    {
        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError($"Insufficient stock for product: {product.name}");
                yield break;
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(ProductData product, System.Action<bool> callback)
    {
        string stockCheckUrl = $"http://192.168.137.75:5000/api/products/checkStock/{product.name}?quantity={product.quantity}";
        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error during stock check request: {stockRequest.error}");
            callback(false);
            yield break;
        }

        StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
        callback(stockResponse.stock >= product.quantity);
    }

    IEnumerator SendOrder()
    {
        Order order = new Order
        {
            products = products
        };

        string jsonOrder = JsonConvert.SerializeObject(order);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
        }
        else
        {
            Debug.LogError($"Error sending order: {request.error}");
        }
    }

    [System.Serializable]
    public class Order
    {
        public List<ProductData> products;
    }

    [System.Serializable]
    public class ProductData
    {
        public string name;
        public int quantity;
    }

    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}
*/


using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class OrderSender : MonoBehaviour
{
    private string apiUrl = "http://192.168.137.75:5000/api/orders";

    public Cart cart; // Reference to the Cart
    public Button sendOrderButton;

    void Start()
    {
        if (sendOrderButton != null)
        {
            sendOrderButton.onClick.AddListener(OnSendOrderButtonClick);
        }
        else
        {
            Debug.LogError("Send Order button is not assigned in the inspector!");
        }
    }

    public void OnSendOrderButtonClick()
    {
        Debug.Log("Send Order button clicked!");
        PrepareOrderFromCart();
        StartCoroutine(CheckStockBeforeOrder());
    }

    private List<ProductData> products = new List<ProductData>();

    void PrepareOrderFromCart()
    {
        if (cart != null)
        {
            List<Product> cartProducts = cart.GetProducts();
            UpdateProductsFromCart(cartProducts);
        }
        else
        {
            Debug.LogError("Cart is not assigned!");
        }
    }

    void UpdateProductsFromCart(List<Product> cartProducts)
    {
        products.Clear();
        foreach (Product product in cartProducts)
        {
            products.Add(new ProductData { name = product.name, quantity = product.quantity });
        }
        Debug.Log("Updated product list from cart.");
    }

    IEnumerator CheckStockBeforeOrder()
    {
        foreach (var product in products)
        {
            bool isStockAvailable = false;

            yield return StartCoroutine(CheckStockAvailability(product, (available) =>
            {
                isStockAvailable = available;
            }));

            if (!isStockAvailable)
            {
                Debug.LogError($"Insufficient stock for product: {product.name}");
                yield break;
            }
        }

        Debug.Log("All products are in stock. Sending order...");
        StartCoroutine(SendOrder());
    }

    IEnumerator CheckStockAvailability(ProductData product, System.Action<bool> callback)
    {
        string stockCheckUrl = $"http://192.168.137.75:5000/api/products/checkStock/{product.name}?quantity={product.quantity}";
        UnityWebRequest stockRequest = UnityWebRequest.Get(stockCheckUrl);

        yield return stockRequest.SendWebRequest();

        if (stockRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error during stock check request: {stockRequest.error}");
            callback(false);
            yield break;
        }

        StockResponse stockResponse = JsonConvert.DeserializeObject<StockResponse>(stockRequest.downloadHandler.text);
        callback(stockResponse.stock >= product.quantity);
    }

    IEnumerator SendOrder()
    {
        Order order = new Order
        {
            products = products
        };

        string jsonOrder = JsonConvert.SerializeObject(order);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonOrder);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order sent successfully!");
        }
        else
        {
            Debug.LogError($"Error sending order: {request.error}");
        }
    }

    [System.Serializable]
    public class Order
    {
        public List<ProductData> products;
    }

    [System.Serializable]
    public class ProductData
    {
        public string name;
        public int quantity;
    }

    [System.Serializable]
    public class StockResponse
    {
        public int stock;
    }
}
