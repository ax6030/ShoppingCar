namespace ShoppingCar.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }            //商品名稱
        public string Description { get; set; }     //商品簡介
        public string Content { get; set; }         //商品內容
        public int Price { get; set; }              //商品價格
        public int Stock { get; set; }              //商品庫存
        public byte[] Image { get; set; }           //商品圖片

        public int CategoryId { get; set; }         //類別 (Foreign Key)
        public Category Category { get; set; }
    }
}
