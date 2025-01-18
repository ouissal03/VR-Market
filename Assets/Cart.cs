/*using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    private List<Product> productsInCart = new List<Product>();

    // Exposes the products in the cart
    public List<Product> GetProducts()
    {
        return productsInCart;
    }

    // Detect when a product enters the cart
    private void OnTriggerEnter(Collider other)
    {
        Product product = other.GetComponent<Product>();
        if (product != null && !productsInCart.Contains(product))
        {
            productsInCart.Add(product);
            Debug.Log($"Product added to cart: {product.name}");
        }
    }

    // Detect when a product exits the cart
    private void OnTriggerExit(Collider other)
    {
        Product product = other.GetComponent<Product>();
        if (product != null && productsInCart.Contains(product))
        {
            productsInCart.Remove(product);
            Debug.Log($"Product removed from cart: {product.name}");
        }
    }
}*/

/* LAST WORKING SCRIPT */

using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    private Dictionary<string, Product> productsInCart = new Dictionary<string, Product>();

    // Exposes the products in the cart
    public List<Product> GetProducts()
    {
        return new List<Product>(productsInCart.Values);
    }

    // Detect when a product enters the cart
    private void OnTriggerEnter(Collider other)
    {
        Product product = other.GetComponent<Product>();
        if (product != null)
        {
            if (productsInCart.ContainsKey(product.name))
            {
                // Increment quantity if the product is already in the cart
                productsInCart[product.name].quantity += product.quantity;
                Debug.Log($"Increased quantity of {product.name} to {productsInCart[product.name].quantity}");
            }
            else
            {
                // Add new product entry
                productsInCart[product.name] = new Product
                {
                    name = product.name,
                    quantity = product.quantity
                };
                Debug.Log($"Added product to cart: {product.name}, quantity: {product.quantity}");
            }
        }
    }

    // Detect when a product exits the cart
    private void OnTriggerExit(Collider other)
    {
        Product product = other.GetComponent<Product>();
        if (product != null && productsInCart.ContainsKey(product.name))
        {
            if (productsInCart[product.name].quantity > product.quantity)
            {
                // Decrease the quantity if more than one of the product exists in the cart
                productsInCart[product.name].quantity -= product.quantity;
                Debug.Log($"Decreased quantity of {product.name} to {productsInCart[product.name].quantity}");
            }
            else
            {
                // Remove the product if the quantity drops to zero
                productsInCart.Remove(product.name);
                Debug.Log($"Removed product from cart: {product.name}");
            }
        }
    }
}

