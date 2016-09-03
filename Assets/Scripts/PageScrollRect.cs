using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageScrollRect : ScrollRect {
	// pageNum 1~3    内部処理では 1p:0  2p:-1  3p:-2
	public int pageNum;			// Settingからページ数を設定(フリックでのPage移動で行き過ぎないように制限をかけるため) 0~3
	// 1ページの幅.
	private float pageWidth;
	// 前回のページIndex. 最も左を0とする.   ※contentのアンカーを左端にセットすることが重要！　そうしないと2ページが３ページ扱いになる。
	private int prevPageIndex = 0;
	private bool isDrag = false;
	private float destXX;


	protected override void Awake()
	{
		base.Awake();

		GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();

		// 1ページの幅を取得.
		pageWidth = grid.cellSize.x + grid.spacing.x;
	}

	// ドラッグを開始したとき.
	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		isDrag = true;
	}

	// ドラッグを終了したとき.
	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		isDrag = false;

		// ドラッグを終了したとき、スクロールをとめます.
		// スナップさせるページが決まった後も慣性が効いてしまうので.
		StopMovement();

		// スナップさせるページを決定する.
		// スナップさせるページのインデックスを決定する.
		int pageIndex = Mathf.RoundToInt(content.anchoredPosition.x / pageWidth);
		GameManager.currentPage = pageIndex;
		// ページが変わっていない且つ、素早くドラッグした場合.
		// ドラッグ量の具合は適宜調整してください.
/*		if (pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 205)	// 空白ページに行くのでページ移動させない
		{
			pageIndex += (int)Mathf.Sign(eventData.delta.x);
		}*/
		if (pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 3)
		{
			int movePage = (int)Mathf.Sign(eventData.delta.x);
			if (pageIndex + movePage <= 0 && pageIndex + movePage > pageNum*(-1))
				pageIndex += (int)Mathf.Sign(eventData.delta.x);
		}

		// Contentをスクロール位置を決定する.
		// 必ずページにスナップさせるような位置になるところがポイント.
		float destX = pageIndex * pageWidth;
		destXX = destX;
//		content.anchoredPosition = new Vector2(destX, content.anchoredPosition.y);

		// 「ページが変わっていない」の判定を行うため、前回スナップされていたページを記憶しておく.
		prevPageIndex = pageIndex;
	}

	new void Update() {
		if (!isDrag && content.anchoredPosition.x != destXX) {
			float newx = Mathf.Lerp (content.anchoredPosition.x, destXX, Time.deltaTime * 7f);		//値が大きいと早くなる
			content.anchoredPosition = new Vector2 (newx, content.anchoredPosition.y);
			GameManager.currentPage = (int)(destXX / pageWidth);
		}
	}


	private int sevePage;
	public void saveCurrentPage() {
		sevePage = prevPageIndex;
		content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
	}
	public void restoreCurrentPage() {
		float destX = sevePage * pageWidth;
		content.anchoredPosition = new Vector2(destX, content.anchoredPosition.y);
		GameManager.currentPage = sevePage;
	}
	public int getPage() {
		return prevPageIndex;
	}
	public void setPage(int page) {
		destXX = page * pageWidth;
		prevPageIndex = page;
		GameManager.currentPage = page;
		// ページ移動はUpdateのLeap処理で行う
	}
}
