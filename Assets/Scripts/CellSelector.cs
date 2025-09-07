using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles cell selection and highlighting functionality
/// </summary>
public class CellSelector : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask cellLayerMask = 1;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.blue;
    [SerializeField] private float highlightIntensity = 0.5f;
    
    [Header("UI References")]
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private Transform selectionIndicatorParent;
    
    // Current selection state
    private CellLogic currentlySelectedCell;
    private CellLogic hoveredCell;
    private Renderer selectionIndicatorRenderer;
    
    // Events
    public System.Action<CellLogic> OnCellSelected;
    public System.Action<CellLogic> OnCellHovered;
    public System.Action OnCellDeselected;
    
    private void Start()
    {
        InitializeSelectionIndicator();
    }
    
    private void Update()
    {
        HandleMouseInput();
        UpdateHoverHighlight();
    }
    
    /// <summary>
    /// Initialize the selection indicator
    /// </summary>
    private void InitializeSelectionIndicator()
    {
        if (selectionIndicator == null)
        {
            // Create a simple selection indicator if none provided
            selectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Quad);
            selectionIndicator.name = "SelectionIndicator";
            selectionIndicator.layer = LayerMask.NameToLayer("UI");
            
            // Remove collider from indicator
            Destroy(selectionIndicator.GetComponent<Collider>());
        }
        
        if (selectionIndicatorParent != null)
        {
            selectionIndicator.transform.SetParent(selectionIndicatorParent);
        }
        
        selectionIndicatorRenderer = selectionIndicator.GetComponent<Renderer>();
        if (selectionIndicatorRenderer != null)
        {
            // Set up material for highlighting
            Material highlightMaterial = new Material(Shader.Find("Standard"));
            highlightMaterial.color = highlightColor;
            highlightMaterial.SetFloat("_Metallic", 0f);
            highlightMaterial.SetFloat("_Smoothness", 0.5f);
            selectionIndicatorRenderer.material = highlightMaterial;
        }
        
        // Initially hide the indicator
        selectionIndicator.SetActive(false);
    }
    
    /// <summary>
    /// Handle mouse input for cell selection
    /// </summary>
    private void HandleMouseInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cellLayerMask))
        {
            CellLogic cell = hit.collider.GetComponent<CellLogic>();
            if (cell != null)
            {
                // Update hovered cell
                if (hoveredCell != cell)
                {
                    hoveredCell = cell;
                    OnCellHovered?.Invoke(cell);
                }
                
                // Handle click
                if (Input.GetMouseButtonDown(0))
                {
                    SelectCell(cell);
                }
            }
            else
            {
                ClearHover();
            }
        }
        else
        {
            ClearHover();
            
            // Deselect if clicking outside
            if (Input.GetMouseButtonDown(0))
            {
                DeselectCell();
            }
        }
    }
    
    /// <summary>
    /// Update hover highlighting
    /// </summary>
    private void UpdateHoverHighlight()
    {
        if (hoveredCell != null && hoveredCell != currentlySelectedCell)
        {
            ShowHoverHighlight(hoveredCell);
        }
    }
    
    /// <summary>
    /// Clear hover state
    /// </summary>
    private void ClearHover()
    {
        if (hoveredCell != null)
        {
            ClearHoverHighlight(hoveredCell);
            hoveredCell = null;
        }
    }
    
    /// <summary>
    /// Select a cell
    /// </summary>
    public void SelectCell(CellLogic cell)
    {
        if (currentlySelectedCell != cell)
        {
            // Deselect previous cell
            if (currentlySelectedCell != null)
            {
                ClearSelectionHighlight(currentlySelectedCell);
            }
            
            currentlySelectedCell = cell;
            ShowSelectionHighlight(cell);
            OnCellSelected?.Invoke(cell);
        }
    }
    
    /// <summary>
    /// Deselect current cell
    /// </summary>
    public void DeselectCell()
    {
        if (currentlySelectedCell != null)
        {
            ClearSelectionHighlight(currentlySelectedCell);
            currentlySelectedCell = null;
            OnCellDeselected?.Invoke();
        }
    }
    
    /// <summary>
    /// Show hover highlight on a cell
    /// </summary>
    private void ShowHoverHighlight(CellLogic cell)
    {
        if (cell != null)
        {
            cell.SetHighlightColor(highlightColor);
        }
    }
    
    /// <summary>
    /// Clear hover highlight from a cell
    /// </summary>
    private void ClearHoverHighlight(CellLogic cell)
    {
        if (cell != null && cell != currentlySelectedCell)
        {
            cell.ClearHighlight();
        }
    }
    
    /// <summary>
    /// Show selection highlight on a cell
    /// </summary>
    private void ShowSelectionHighlight(CellLogic cell)
    {
        if (cell != null)
        {
            cell.SetHighlightColor(selectedColor);
            
            // Position selection indicator
            if (selectionIndicator != null)
            {
                selectionIndicator.transform.position = cell.transform.position + Vector3.up * 0.1f;
                selectionIndicator.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// Clear selection highlight from a cell
    /// </summary>
    private void ClearSelectionHighlight(CellLogic cell)
    {
        if (cell != null)
        {
            cell.ClearHighlight();
            
            // Hide selection indicator
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Get currently selected cell
    /// </summary>
    public CellLogic GetSelectedCell()
    {
        return currentlySelectedCell;
    }
    
    /// <summary>
    /// Get currently hovered cell
    /// </summary>
    public CellLogic GetHoveredCell()
    {
        return hoveredCell;
    }
    
    /// <summary>
    /// Check if a cell is currently selected
    /// </summary>
    public bool IsCellSelected(CellLogic cell)
    {
        return currentlySelectedCell == cell;
    }
    
    /// <summary>
    /// Set the highlight color for selection
    /// </summary>
    public void SetHighlightColor(Color color)
    {
        highlightColor = color;
    }
    
    /// <summary>
    /// Set the selected color
    /// </summary>
    public void SetSelectedColor(Color color)
    {
        selectedColor = color;
    }
}
